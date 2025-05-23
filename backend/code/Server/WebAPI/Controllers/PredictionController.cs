using System.Text;
using System.Text.Json;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PredictionController : ControllerBase
{
    private readonly IPredictionInterface _predictionInterface;
    private readonly ILogger<PredictionController> _logger;
    private readonly HttpClient _mlClient;

    public PredictionController(
        IPredictionInterface predictionInterface,
        ILogger<PredictionController> logger,
        IHttpClientFactory httpFactory
    )
    {
        _predictionInterface = predictionInterface;
        _logger = logger;
        _mlClient = httpFactory.CreateClient("ml-api");
    }

    /*[HttpGet("{id}")]
    public async Task<ActionResult<Prediction>> GetPredictionById(int id)
    {
        var prediction = await _predictionInterface.GetPredictionByIdAsync(id);

        if (prediction == null)
        {
            return NotFound($"No prediction found with id {id}");
        }

        return Ok(prediction);
    }

    [HttpGet("date/{date}")]
    public async Task<ActionResult<List<Prediction>>> GetPredictionsByDate(DateTime date)
    {
        var predictions = await _predictionInterface.GetPredictionsByDateAsync(date);
        return Ok(predictions);
    }

    [HttpGet]
    public async Task<ActionResult<List<Prediction>>> GetAllPredictions()
    {
        var predictions = await _predictionInterface.GetAllPredictions();
        return Ok(predictions);
    }

    [HttpPost]
    public async Task<ActionResult> AddPrediction([FromBody] PredictionDTO prediction)
    {
        if (prediction == null)
        {
            return BadRequest("Prediction cannot be null");
        }

        await _predictionInterface.AddPredictionAsync(prediction);
        return Ok(prediction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePrediction(int id)
    {
        var existing = await _predictionInterface.GetPredictionByIdAsync(id);
       
        if (existing == null)
        {
            return NotFound($"No prediction found with id {id}");
        }

        await _predictionInterface.DeletePredictionAsync(id);
        return Ok($"Prediction with id {id} deleted");
    }*/

    /// <summary>
    /// GET api/prediction/repredict/latest
    /// Retrieves the latest saved prediction, re-sends it to the Python ML endpoint,
    /// and returns the updated prediction.
    /// </summary>
    [HttpPost("repredict/latest")]
    public async Task<ActionResult<PredictionResponseDTO>> RepredictLatest()
    {
        var rawPrediction = await _predictionInterface.GetLastPredictionAsync();

        var payload = new
        {
            temperature = rawPrediction.Temperature,
            light = rawPrediction.Light,
            airHumidity = rawPrediction.AirHumidity,
            soilHumidity = rawPrediction.SoilHumidity,
            date = rawPrediction.Date, // Make sure this is a DateTime
            greenhouseId = rawPrediction.GreenhouseId,
            sensorReadingId = rawPrediction.SensorReadingId
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = null };

        var json = JsonSerializer.Serialize(payload, options);
        Console.WriteLine("Sending JSON:\n" + json);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _mlClient.PostAsync("/predict", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PredictionResponseDTO>(
            responseJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return result!;
    }
}
