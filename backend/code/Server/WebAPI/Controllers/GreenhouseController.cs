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

    [HttpGet("{id}")]
    public async Task<ActionResult<Greenhouse>> GetGreenhouseByIdAsync(int id)
    {
        var greenhouse = await _greenhouse.GetGreenhouseByIdAsync(id);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with id {id} not found.");
        }
        return Ok(greenhouse);
    }

    [HttpGet("gardener/{gardenerId}")]
    public async Task<ActionResult<Greenhouse>> GetGreenhouseByGardenerId(int gardenerId)
    {
        var greenhouse = await _greenhouse.GetGreenhouseByGardenerIdAsync(gardenerId);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with gardenerId {gardenerId} does not exist.");
        }

        return Ok(greenhouse);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<Greenhouse>> GetGreenhouseByNameAsync(string name)
    {
        var greenhouse = await _greenhouse.GetGreenhouseByNameAsync(name);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with name {name} does not exist.");
        }
        return Ok(greenhouse);
    }

    [HttpPost]
    public async Task<ActionResult<Greenhouse>> AddGreenhouseAsync([FromBody] GreenhouseDTO greenhouse)
    {
        if (greenhouse == null)
        {
            return BadRequest($"Gardener data is required.");
        }

        var addedGreenhouse = await _greenhouse.AddGreenhouseAsync(greenhouse);
        return Ok(addedGreenhouse);
    }

    [HttpPatch]
    public async Task<ActionResult<Greenhouse>> UpdateGreenhouseAsync([FromBody] Greenhouse greenhouse)
    {
        if (greenhouse == null)
        {
            return BadRequest($"Gardener data is required.");
        }

        var updatedGreenhouse = await _greenhouse.UpdateGreenhouseAsync(greenhouse);
        if (updatedGreenhouse == null)
        {
            return NotFound($"Greenhouse with id {greenhouse.Id} not found.");
        }
        return Ok(updatedGreenhouse);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGreenhouseAsync(int id)
    {
        var greenhouse = await _greenhouse.GetGreenhouseByIdAsync(id);
        if (greenhouse == null)
        {
            return NotFound($"Greenhouse with id {id} not found.");
        }
        await _greenhouse.DeleteGreenhouseAsync(id);
        return NoContent();
    }
}