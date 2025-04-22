namespace DTOs;

public class SensorReadingDTO
{
    public int Value { get; set; } // not sure what this should be
    public DateTime TimeStamp { get; set; }
    public int ThresholdValue { get; set; }

    public int SensorId { get; set; }
}