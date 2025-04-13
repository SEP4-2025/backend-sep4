using Microsoft.Extensions.Diagnostics.HealthChecks;
using MQTTnet;

namespace ReceiverService;

public class SensorReceiverService : BackgroundService, IHealthCheck
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientFactory _mqttFactory = new();
    private readonly List<string> _topics;
    private readonly string _server;
    private readonly int _port;
    private readonly ILogger<SensorReceiverService> _logger;
    private bool _isHealthy = false;

    public SensorReceiverService(
        IConfiguration configuration,
        ILogger<SensorReceiverService> logger
    )
    {
        _mqttClient = _mqttFactory.CreateMqttClient();
        _logger = logger;

        _server =
            configuration["MQTT_BROKER_HOST"]
            ?? throw new ArgumentNullException("MQTT_BROKER_HOST is not set in configuration");

        if (!int.TryParse(configuration["MQTT_BROKER_PORT"], out _port))
        {
            _port = 1883;
        }

        _topics = new List<string>();
        var topicsConfig = configuration["MQTT_TOPICS"];
        foreach (var topic in topicsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            _topics.Add(topic.Trim());
        }

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
