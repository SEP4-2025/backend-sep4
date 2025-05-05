using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class PlantController : ControllerBase
{
    private readonly IPlantInterface _plantInterface;

    public PlantController(IPlantInterface plantInterface)
    {
        _plantInterface = plantInterface;
    }

    [HttpGet("{plantId}")]
    public async Task<ActionResult<Plant>> GetPlantById(int plantId)
    {
        var plant = await _plantInterface.GetPlantByIdAsync(plantId);
        if (plant == null) return NotFound($"No plant found with id {plantId}");
        return Ok(plant);
    }

    [HttpPost]
    public async Task<ActionResult<Plant>> AddPlant([FromBody] PlantDTO plant)
    {
        if (plant == null) return BadRequest($"Plant cannot be null");
        var addedPlant = await _plantInterface.AddPlantAsync(plant);
        return CreatedAtAction(nameof(GetPlantById), new { id = addedPlant.Id }, addedPlant);
    }

    [HttpPut]
    public async Task<ActionResult<Plant>> UpdatePlantName(int id, string plantName)
    {
        var plant = await _plantInterface.GetPlantByIdAsync(id);
        if (plant == null) return NotFound($"No plant found with id {id}");
        await _plantInterface.UpdatePlantNameAsync(id, plantName);
        return Ok(plant);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePlant(int id)
    {
        var plant = await _plantInterface.GetPlantByIdAsync(id);
        if (plant == null) return NotFound($"No plant found with id {id}");

        await _plantInterface.DeletePlantAsync(id);
        return NoContent();
    }
}