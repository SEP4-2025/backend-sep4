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

    public PredictionController(IPredictionInterface predictionInterface)
    {
        _predictionInterface = predictionInterface;
    }

    [HttpGet("{id}")]
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
    }
}
