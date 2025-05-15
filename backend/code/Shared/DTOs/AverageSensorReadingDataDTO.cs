namespace DTOs;

public class AverageSensorReadingDataDTO
{
    public int SensorId { get; set; }
    public string SensorType { get; set; }
    public string MetricUnit { get; set; }
    public double? AverageValue { get; set; }
}
