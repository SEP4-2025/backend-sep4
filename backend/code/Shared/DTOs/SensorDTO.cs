namespace DTOs;

public class SensorDTO
{
    public string Type { get; set; }
    
    public int ThresholdValue { get; set; }
    public string MetricUnit { get; set; }
    public int greenHouseId { get; set; }
}