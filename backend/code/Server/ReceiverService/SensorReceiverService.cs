using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Database;
using DTOs;
using Entities;
using LogicImplements;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MQTTnet;

namespace ReceiverService;

public class SensorReceiverService : BackgroundService, IHealthCheck, IWateringService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientFactory _mqttFactory = new();
    private readonly List<string> _topics;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string? _webApiEndpoint;
    private readonly string _server;
    private readonly int _port;
    private readonly ILogger<SensorReceiverService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private bool _isHealthy = false;

    private class SensorState
    {
        public double Temperature { get; set; }
        public double Light { get; set; }
        public double AirHumidity { get; set; }
        public double SoilHumidity { get; set; }
    }
    private readonly ConcurrentDictionary<int, SensorState> _latestState
        = new ConcurrentDictionary<int, SensorState>();
    public SensorReceiverService(
        ILogger<SensorReceiverService> logger,
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration
    )
    {
        _mqttClient = _mqttFactory.CreateMqttClient();
        _logger = logger;
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;

        _server = "34.27.128.90";
        _port = 1883;

        // Only for Proof of concept, will be in .ENV file in future
        // Define topics to subscribe to
        _topics = new List<string>
        {
            "light/reading",
            "air/temperature",
            "air/humidity",
            "soil/reading",
        };

        _webApiEndpoint = configuration["CloudWebApiEndpoint"];

        _logger.LogInformation(
            "ReceiverService configured with MQTT broker at {Server}:{Port}",
            _server,
            _port
        );
        _logger.LogInformation("Subscribed to topics: {Topics}", string.Join(", ", _topics));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ReceiverUtil.ConfigureMqttClientEvents(
            _mqttClient,
            _logger,
            HandleSensorReading,
            isConnected => _isHealthy = isConnected
        );

        int reconnectAttempts = 0;
        const int maxReconnectDelay = 60000; // 1 minute max delay

        try
        {
            await ConnectAndSubscribe(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_mqttClient.IsConnected)
                    {
                        _isHealthy = false;

                        // Calculate backoff delay with exponential backoff
                        int delayMs = Math.Min(
                            (int)(1000 * Math.Pow(2, reconnectAttempts)),
                            maxReconnectDelay
                        );
                        reconnectAttempts++;

                        _logger.LogWarning(
                            "Connection lost. Attempting to reconnect in {DelaySeconds} seconds (attempt {Attempt})...",
                            delayMs / 1000,
                            reconnectAttempts
                        );

                        await Task.Delay(delayMs, stoppingToken);
                        await ConnectAndSubscribe(stoppingToken);

                        if (_mqttClient.IsConnected)
                        {
                            // Reset reconnect counter on successful connection
                            reconnectAttempts = 0;
                        }
                    }
                    else
                    {
                        // If connected, reset reconnect counter and wait
                        reconnectAttempts = 0;
                        await Task.Delay(5000, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Service shutdown requested, exiting monitoring loop");
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Service shutdown requested during operation");
        }
        catch (Exception ex)
        {
            _isHealthy = false;
            _logger.LogError(ex, "Error in ExecuteAsync");
        }
        finally
        {
            await ReceiverUtil.DisconnectMqttClient(_mqttClient, _logger, CancellationToken.None);
        }
    }

    private async Task HandleSensorReading(SensorReadingDTO sensorReading)
    {
        try
        {
            /*using var scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var sensorReadingLogic = new SensorReadingLogic(dbContext);

            try
            {
                _logger.LogInformation("Adding sensor reading to database...");
                await sensorReadingLogic.AddSensorReadingAsync(sensorReading);
                _logger.LogInformation(
                    "Sensor reading added to database: {SensorReading}",
                    JsonSerializer.Serialize(sensorReading)
                );
            }
            catch
            {
                _logger.LogError("Failed to add sensor reading to database");
                return;
            }

            // Notification logic v2
            await HandlePostNotifications(sensorReading);*/
            // Update state and save prediction
            await UpdateStateAndSavePredictionAsync(sensorReading);

            _logger.LogInformation("Successfully added sensor reading to database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add sensor reading to database");
        }
    }

    private async Task ConnectAndSubscribe(CancellationToken stoppingToken)
    {
        // Use a stable client ID to prevent constant reconnections
        string clientId =
            $"GrowMateSensorReceiverService-{Guid.NewGuid().ToString().Substring(0, 6)}";

        bool connected = await ReceiverUtil.ConnectMqttClient(
            _mqttClient,
            _server,
            _port,
            clientId,
            _logger,
            stoppingToken
        );

        if (connected)
        {
            bool subscribed = await ReceiverUtil.SubscribeToTopics(
                _mqttClient,
                _mqttFactory,
                _topics,
                _logger,
                stoppingToken
            );

            _isHealthy = subscribed;
        }
        else
        {
            _isHealthy = false;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync called, shutting down service...");

        await ReceiverUtil.DisconnectMqttClient(_mqttClient, _logger, cancellationToken);

        await base.StopAsync(cancellationToken);
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        if (_isHealthy && _mqttClient.IsConnected)
        {
            return Task.FromResult(HealthCheckResult.Healthy("MQTT client is connected"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("MQTT client is not connected"));
    }

    public async Task HandlePostNotifications(SensorReadingDTO sensorReading)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var waterPumpLogic = new WaterPumpLogic(dbContext);
        var sensorLogic = new SensorLogic(dbContext);

        var sensor = await sensorLogic.GetSensorByIdAsync(sensorReading.SensorId);

        switch (sensor.Type)
        {
            case "Light":
                if (
                    sensorReading.Value > sensor.ThresholdValue + 150
                    || sensorReading.Value < sensor.ThresholdValue - 150
                )
                {
                    await CreateNotification(sensorReading);
                }
                break;
            case "Temperature":
                if (
                    sensorReading.Value > sensor.ThresholdValue + 4
                    || sensorReading.Value < sensor.ThresholdValue - 4
                )
                {
                    await CreateNotification(sensorReading);
                }

                break;
            case "Humidity":
                if (
                    sensorReading.Value > sensor.ThresholdValue + 7
                    || sensorReading.Value < sensor.ThresholdValue - 7
                )
                {
                    await CreateNotification(sensorReading);
                }
                break;
            case "Soil Moisture":
                var waterPump = await waterPumpLogic.GetWaterPumpByIdAsync(sensorReading.SensorId);

                if (
                    sensorReading.Value > sensor.ThresholdValue + 20
                    || sensorReading.Value < sensor.ThresholdValue - 20
                )
                {
                    await CreateNotification(sensorReading);
                }
                else if (sensorReading.Value < 40 && waterPump.AutoWateringEnabled)
                {
                    await TriggerWateringAsync(waterPump.ThresholdValue);
                }
                else if (waterPump.LastWateredTime < DateTime.UtcNow.AddHours(-24) && waterPump.AutoWateringEnabled)
                {
                    await TriggerWateringAsync(waterPump.ThresholdValue);
                }
                break;
        }
    }

    public async Task TriggerWateringAsync(int waterAmount)
    {
        //Connect to MQTT broker
        if (!_mqttClient.IsConnected)
        {
            string clientId =
                $"GrowMateSensorReceiverService-{Guid.NewGuid().ToString().Substring(0, 6)}";

            await ReceiverUtil.ConnectMqttClient(
                _mqttClient,
                _server,
                _port,
                clientId,
                _logger,
                CancellationToken.None
            );
            _logger.LogInformation("Sender is now sending pump data...");
        }

        if (!_mqttClient.IsConnected)
        {
            _logger.LogError("Failed to connect to MQTT broker");
            return;
        }

        // If 250ml = 3000ms, then 1ml = 12ms
        var ms = waterAmount * 12;

        var topic = "pump/command";

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(ms.ToString())
            .Build();

        await _mqttClient.PublishAsync(message);
        _logger.LogInformation("Published watering command to topic {Topic}: {Payload}ml ({Duration}ms)", topic, waterAmount, ms);
    }

    public async Task CreateNotification(SensorReadingDTO sensorReading)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sensorLogic = new SensorLogic(dbContext);

        var sensor = await sensorLogic.GetSensorByIdAsync(sensorReading.SensorId);

        var notificationPayload = new NotificationDTO
        {
            SensorId = sensorReading.SensorId,
            Message =
                "Warning: "
                + sensorReading.Value.ToString()
                + sensor.MetricUnit
                + " is deviation from the set norm",
            TimeStamp = sensorReading.TimeStamp,
            Type = sensor.Type,
        };
        var httpClient = _httpClientFactory.CreateClient();
        var json = JsonSerializer.Serialize(notificationPayload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        _logger.LogInformation("Sending notification to Web API: {Payload}", json);
        try
        {
            var response = await httpClient.PostAsync(_webApiEndpoint, content);
            _logger.LogInformation(
                "Notification sent to Web API: {StatusCode}",
                response.StatusCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification");
        }
    }
    private async Task UpdateStateAndSavePredictionAsync(SensorReadingDTO sensorReading)
    {
        // create a scope so we can resolve SensorLogic + DbContext
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sensorLogic = new SensorLogic(dbContext);
        var sensor = await sensorLogic.GetSensorByIdAsync(sensorReading.SensorId);

        // get or create the state object for this greenhouse
        var state = _latestState.GetOrAdd(
            sensor.GreenhouseId,
            _ => new SensorState()
        );

        bool changed = false;

        // update only the matching field, mark if it actually changed
        switch (sensor.Type)
        {
            case "Temperature":
                if (state.Temperature != sensorReading.Value)
                {
                    state.Temperature = sensorReading.Value;
                    changed = true;
                }
                break;

            case "Humidity":
                if (state.AirHumidity != sensorReading.Value)
                {
                    state.AirHumidity = sensorReading.Value;
                    changed = true;
                }
                break;

            case "Light":
                if (state.Light != sensorReading.Value)
                {
                    state.Light = sensorReading.Value;
                    changed = true;
                }
                break;

            case "Soil Moisture":
                if (state.SoilHumidity != sensorReading.Value)
                {
                    state.SoilHumidity = sensorReading.Value;
                    changed = true;
                }
                break;
        }

        if (!changed)
        {
            // nothing to do if the value didn't move
            return;
        }

        if (!changed)
            return;


        if (state.Temperature is double temp &&
            state.AirHumidity is double humid &&
            state.Light is double light &&
            state.SoilHumidity is double soil)
        {

            var prediction = new Prediction
            {
                Temperature = (int)temp,
                AirHumidity = (int)humid,
                Light = (int)light,
                SoilHumidity = (int)soil,
                Date = sensorReading.TimeStamp,
                GreenhouseId = sensor.GreenhouseId,
                SensorReadingId = 4
            };

            try
            {
                await dbContext.Predictions.AddAsync(prediction);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Persisted updated Prediction: {@prediction}", prediction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save updated Prediction");
            }
        }
    }

}