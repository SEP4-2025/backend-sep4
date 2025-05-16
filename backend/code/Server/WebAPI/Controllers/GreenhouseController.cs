using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class GreenhouseController : ControllerBase
{
    private readonly IGreenhouseInterface _greenhouse;

    public GreenhouseController(IGreenhouseInterface greenhouse)
    {
        _greenhouse = greenhouse;
    }

    [HttpGet]
    public async Task<ActionResult<List<Greenhouse>>> GetAllGreenhousesAsync()
    {
        var greenhouses = await _greenhouse.GetGreenhouses();
        if (greenhouses == null || !greenhouses.Any())
        {
            return NotFound("No greenhouses found.");
        }
        return Ok(greenhouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Greenhouse>> GetGreenhouseByIdAsync(int id)
    {
        var greenhouse = await _greenhouse.GetGreenhouseById(id);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with id {id} not found.");
        }
        return Ok(greenhouse);
    }

    [HttpGet("gardener/{gardenerId}")]
    public async Task<ActionResult<Greenhouse>> GetGreenhouseByGardenerId(int gardenerId)
    {
        var greenhouse = await _greenhouse.GetGreenhouseByGardenerId(gardenerId);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with gardenerId {gardenerId} does not exist.");
        }

        return Ok(greenhouse);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<Greenhouse>> GetGreenhouseByNameAsync(string name)
    {
        var greenhouse = await _greenhouse.GetGreenhouseByName(name);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with name {name} does not exist.");
        }
        return Ok(greenhouse);
    }

    [HttpPost]
    public async Task<ActionResult<Greenhouse>> AddGreenhouseAsync(
        [FromBody] GreenhouseDTO greenhouse
    )
    {
        if (greenhouse.isEmpty())
        {
            return BadRequest($"Gardener data is required.");
        }

        var greenhouses = await _greenhouse.GetGreenhouses();
        var greenhousesCount = greenhouses.Count();
        if (greenhousesCount > 0)
        {
            return BadRequest("You can only create one greenhouse.");
        }

        var addedGreenhouse = await _greenhouse.AddGreenhouse(greenhouse);
        return Ok(addedGreenhouse);
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult<Greenhouse>> UpdateGreenhouseNameAsync(
        int id,
        [FromBody] string name
    )
    {
        var greenhouse = await _greenhouse.GetGreenhouseById(id);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with id {id} not found.");
        }

        if (string.IsNullOrEmpty(name))
        {
            return BadRequest("Name cannot be empty.");
        }

        await _greenhouse.UpdateGreenhouseName(id, name);

        return Ok(greenhouse);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGreenhouseAsync(int id)
    {
        var greenhouse = await _greenhouse.GetGreenhouseById(id);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with id {id} not found.");
        }

        try
        {
            await _greenhouse.DeleteGreenhouse(id);
        }
        catch (Exception e)
        {
            if (e.InnerException?.Message.Contains("Plant_greenhouseid_fkey") == true)
            {
                return BadRequest(
                    "Cannot delete Greenhouse because it is associated with a Plant."
                );
            }
            if (e.InnerException?.Message.Contains("Sensor_greenhouseid_fkey") == true)
            {
                return BadRequest("Cannot delete Greenhouse because it has associated Sensors.");
            }
            throw;
        }
        return Ok("Successfully deleted greenhouse.");
    }
}
