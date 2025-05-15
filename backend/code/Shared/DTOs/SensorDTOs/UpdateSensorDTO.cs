namespace DTOs;

public class UpdateSensorDTO
{
    public string? Type { get; set; }
    public string? MetricUnit { get; set; }
    public int? ThresholdValue { get; set; }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Type) && string.IsNullOrWhiteSpace(MetricUnit) && ThresholdValue == null;
    }
}