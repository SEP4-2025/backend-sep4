namespace Entities;

public class SensorReading
{
    public int Id { get; set; }
    public string Value { get; set; } // not sure what this should be
    public DateTime TimeStamp { get; set; }

    public int SensorReadingId { get; set; }
}
