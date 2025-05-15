using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class WaterPumpController : ControllerBase
{
    private readonly IWaterPumpInterface _waterPumpLogic;

    public WaterPumpController(IWaterPumpInterface waterPumpLogic)
    {
        _waterPumpLogic = waterPumpLogic;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WaterPump>> GetWaterPumpByIdAsync(int id)
    {
        var pump = await _waterPumpLogic.GetWaterPumpByIdAsync(id);
        if (pump == null)
        {
            return NotFound($"Water pump with id {id} not found.");
        }
        return Ok(pump);
    }

    [HttpGet]
    public async Task<ActionResult<List<WaterPump>>> GetAllWaterPumpsAsync()
    {
        var pumps = await _waterPumpLogic.GetAllWaterPumpsAsync();
        return Ok(pumps);
    }

    [HttpPost]
    public async Task<ActionResult<WaterPump>> AddWaterPumpAsync([FromBody] WaterPumpDTO waterPumpDTO)
    {
        if (waterPumpDTO == null)
        {
            return BadRequest("Water pump data is required.");
        }

        var addedPump = await _waterPumpLogic.AddWaterPumpAsync(waterPumpDTO);
        return Ok(addedPump);
    }

    [HttpPatch("{id}/toggle-automation")]
    public async Task<ActionResult<WaterPump>> ToggleAutomationStatusAsync(int id, [FromBody] bool autoWatering)
    {
        var updatedPump = await _waterPumpLogic.ToggleAutomationStatusAsync(id, autoWatering);
        if (updatedPump == null)
        {
            return NotFound($"Water pump with id {id} not found.");
        }
        return Ok(updatedPump);
    }

    [HttpPatch("{id}/manual-watering")]
    public async Task<ActionResult<WaterPump>> TriggerManualWateringAsync(int id, [FromQuery] int waterAmount)
    {
        var pump = await _waterPumpLogic.GetWaterPumpByIdAsync(id);
        if (pump == null)
        {
            return NotFound($"Water pump with id {id} not found.");
        }
        await _waterPumpLogic.TriggerManualWateringAsync(id, waterAmount);
        return Ok(pump);
    }

    [HttpPatch("{id}/add-water")]
    public async Task<ActionResult<WaterPump>> UpdateCurrentWaterLevelAsync(int id, [FromBody] int addedWaterAmount)
    {
        var updatedPump = await _waterPumpLogic.UpdateCurrentWaterLevelAsync(id, addedWaterAmount);
        if (updatedPump == null)
        {
            return NotFound($"Water pump with id {id} not found.");
        }
        return Ok(updatedPump);
    }

    [HttpPatch("{id}/threshold")]
    public async Task<ActionResult<WaterPump>> UpdateThresholdValueAsync(int id, [FromBody] int newThresholdValue)
    {
        var updatedPump = await _waterPumpLogic.UpdateThresholdValueAsync(id, newThresholdValue);
        if (updatedPump == null)
        {
            return NotFound($"Water pump with id {id} not found.");
        }
        return Ok(updatedPump);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWaterPumpAsync(int id)
    {
        var pump = await _waterPumpLogic.GetWaterPumpByIdAsync(id);
        if (pump == null)
        {
            return NotFound($"Water pump with id {id} not found.");
        }

        await _waterPumpLogic.DeleteWaterPumpAsync(id);
        return NoContent();
    }
}