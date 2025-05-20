namespace Entities;

public class Prediction
{
    public int Id { get; set; }
    public int Temperature { get; set; }
    public int Light { get; set; }
    public int AirHumidity { get; set; }
    public int SoilHumidity { get; set; }
    public DateTime Date { get; set; }

    public int GreenhouseId { get; set; }
    public int SensorReadingId { get; set; }
}
