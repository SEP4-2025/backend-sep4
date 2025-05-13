namespace ReceiverService;

public class MqttWateringService : IMqttWateringService
{
    private readonly SensorReceiverService _sensorReceiverService;
    private readonly ILogger<MqttWateringService> _logger;

    public MqttWateringService(
        SensorReceiverService sensorReceiverService,
        ILogger<MqttWateringService> logger)
    {
        _sensorReceiverService = sensorReceiverService;
        _logger = logger;
    }

    public async Task TriggerWateringAsync(int waterAmount)
    {
        var ms = waterAmount ;
        
        try
        {
            await _sensorReceiverService.TriggerWateringAsync(ms);
            _logger.LogInformation("Triggered watering for {Duration}ms", ms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trigger MQTT watering");
        }
    }
}