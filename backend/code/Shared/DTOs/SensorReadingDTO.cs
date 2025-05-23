namespace DTOs;

public class SensorReadingDTO
{
    public int Value { get; set; }
    public DateTime TimeStamp { get; set; }

    public int SensorId { get; set; }

    public bool IsMissingValues()
    {
        return Value == 0 || TimeStamp == default;
    }
}
