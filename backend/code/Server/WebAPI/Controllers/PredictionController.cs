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

    public PredictionController(IPredictionInterface predictionInterface, ILogger<PredictionController> logger)
    {
        _predictionInterface = predictionInterface;
        _logger = logger;
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
    [HttpGet("repredict/latest")]
    public async Task<ActionResult<PredictionResponseDTO>> RepredictLatest()
    {
        try
        {
            var result = await _predictionInterface.RepredictLatestAsync();
            return Ok(result);
        }
        catch (InvalidOperationException inv)
        {
            return NotFound(inv.Message);
        }
        catch (HttpRequestException httpEx)
        {
            return StatusCode(502, httpEx.Message);
        }
        catch (Exception ex)
        {
            // return full exception message so you can vedea exact de unde vine
            return StatusCode(500, ex.ToString());
        }
    }
}
