namespace Entities;

public class Notification
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
    public bool IsRead { get; set; }

    public int WaterPumpId { get; set; }
    public int SensorReadingId { get; set; }
}