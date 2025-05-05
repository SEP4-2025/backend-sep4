namespace DTOs;

public class AddSensorDTO
{
    public string Type { get; set; }

    public int ThresholdValue { get; set; }
    public string MetricUnit { get; set; }
    public int GreenHouseId { get; set; }
}