using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PlantController : ControllerBase
{
    private readonly IPlantInterface _plantInterface;

    public PlantController(IPlantInterface plantInterface)
    {
        _plantInterface = plantInterface;
    }

    [HttpGet]
    public async Task<ActionResult<List<Plant>>> GetPlants()
    {
        var plants = await _plantInterface.GetPlantsAsync();
        if (plants == null || !plants.Any())
        {
            return NotFound("No plants found.");
        }
        return Ok(plants);
    }

    [HttpGet("{plantId}")]
    public async Task<ActionResult<Plant>> GetPlantById(int plantId)
    {
        var plant = await _plantInterface.GetPlantByIdAsync(plantId);
        if (plant == null)
            return NotFound($"No plant found with id {plantId}");
        return Ok(plant);
    }

    [HttpPost]
    public async Task<ActionResult<Plant>> AddPlant([FromBody] PlantDTO plant)
    {
        if (plant.IsEmpty())
            return BadRequest($"Plant cannot be null");
        var plants = await _plantInterface.GetPlantsAsync();
        var plantCount = plants.Count();
        if (plantCount > 0)
        {
            return BadRequest("You can only create one plant.");
        }
        var addedPlant = await _plantInterface.AddPlantAsync(plant);
        return Ok(addedPlant);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Plant>> UpdatePlantName(int id, [FromBody] string plantName)
    {
        var plant = await _plantInterface.GetPlantByIdAsync(id);
        if (plant == null)
            return NotFound($"No plant found with id {id}");

        if (string.IsNullOrEmpty(plantName))
        {
            return BadRequest("Plant name cannot be null or empty.");
        }
        await _plantInterface.UpdatePlantNameAsync(id, plantName);
        return Ok(plant);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePlant(int id)
    {
        var plant = await _plantInterface.GetPlantByIdAsync(id);
        if (plant == null)
            return NotFound($"No plant found with id {id}");

        await _plantInterface.DeletePlantAsync(id);
        return Ok();
    }
}
