using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Database;
using DTOs;
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
                        _logger.LogWarning("Connection lost. Attempting to reconnect...");
                        await ConnectAndSubscribe(stoppingToken);
                    }

                    await Task.Delay(5000, stoppingToken);
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
            await Task.Delay(5000);
        }
    }

    private async Task HandleSensorReading(SensorReadingDTO sensorReading)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var sensorReadingLogic = new SensorReadingLogic(dbContext);

            await sensorReadingLogic.AddSensorReadingAsync(sensorReading);

            // Notification logic v2
            await HandlePostNotifications(sensorReading);

            _logger.LogInformation("Successfully added sensor reading to database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add sensor reading to database");
        }
    }

    private async Task ConnectAndSubscribe(CancellationToken stoppingToken)
    {
        string clientId = $"SensorReceiver_{Guid.NewGuid().ToString("N")}";

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
        // If 250ml = 3000ms, then 1ml = 12ms
        var ms = waterAmount * 12;

        if (_mqttClient.IsConnected != true)
        {
            _logger.LogWarning("MQTT client is not connected. Cannot trigger watering.");
            return;
        }

        var topic = $"pump/command";

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
}