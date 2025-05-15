namespace ReceiverService;

public interface IWateringService
{
    Task TriggerWateringAsync(int waterAmount);
}