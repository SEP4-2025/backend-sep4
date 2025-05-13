namespace ReceiverService;

public interface IMqttWateringService
{
    Task TriggerWateringAsync(int durationMs);
}