using System.Text.Json.Serialization;

namespace DTOs;

public class PredictionRequestDTO
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("light")]
    public double Light { get; set; }

    [JsonPropertyName("airHumidity")]
    public double AirHumidity { get; set; }

    [JsonPropertyName("soilHumidity")]
    public double SoilHumidity { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("greenhouseId")]
    public int GreenhouseId { get; set; }

    [JsonPropertyName("sensorReadingId")]
    public int SensorReadingId { get; set; }
}