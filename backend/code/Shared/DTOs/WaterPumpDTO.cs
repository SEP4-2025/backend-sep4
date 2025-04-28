namespace DTOs;

public class WaterPumpDTO
{
    public DateTime LastWatered { get; set; }
    public int LastWaterAmount { get; set; }
    public bool AutoWatering { get; set; }
    public int WaterTankCapacity { get; set; }
    public int CurrentWaterLevel { get; set; }
    public int ThresholdValue { get; set; }
}