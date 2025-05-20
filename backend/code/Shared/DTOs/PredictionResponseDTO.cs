using System.Text.Json.Serialization;

namespace DTOs;

public class PredictionResponseDTO
{
    [JsonPropertyName("prediction")]
    public double Prediction { get; set; }

    [JsonPropertyName("prediction_proba")]
    public List<double>? PredictionProba { get; set; }

    [JsonPropertyName("input_received")]
    public PredictionRequestDTO InputReceived { get; set; } = default!;
}