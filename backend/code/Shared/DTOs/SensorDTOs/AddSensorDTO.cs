namespace DTOs;

public class AddSensorDTO
{
    public string Type { get; set; }
    public string MetricUnit { get; set; }
    public int GreenHouseId { get; set; }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Type) || string.IsNullOrWhiteSpace(MetricUnit) || GreenHouseId <= 0;
    }
}