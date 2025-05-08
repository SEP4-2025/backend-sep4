using System.Text.Json;
using Database;
using DTOs;
using LogicImplements;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MQTTnet;

namespace ReceiverService;

public class SensorReceiverService : BackgroundService, IHealthCheck
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientFactory _mqttFactory = new();
    private readonly List<string> _topics;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _webApiEndpoint;
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

        _server = "35.204.67.247";
        _port = 1883;

        // Only for Proof of concept, will be in .ENV file in future
        // Define topics to subscribe to
        _topics = new List<string>
        {
            "light/reading",
            "temperature/reading",
            "humidity/reading",
            "soil/reading",
        };

        _webApiEndpoint = configuration["WebApiEndpoint"]; //Use CloudWebAPIEnpoint from appsettings.json when testing deployment

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

            // Notification logic v1
            var notificationPayload = new NotificationDTO
            {
                SensorId = sensorReading.SensorId,
                Message = "sensor read: " + sensorReading.Value.ToString(),
                TimeStamp = sensorReading.TimeStamp,
            };
            var httpClient = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(notificationPayload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            try
            {
                var response = await httpClient.PostAsync(_webApiEndpoint, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification");
            }

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
}
