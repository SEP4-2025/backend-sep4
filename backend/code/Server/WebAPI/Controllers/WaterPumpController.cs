using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using ReceiverService;
using Tools;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class WaterPumpController : ControllerBase
{
    private readonly IWaterPumpInterface _waterPumpLogic;
    private readonly INotificationService _notificationService;
    private readonly IWateringService _wateringService;

    public WaterPumpController(IWaterPumpInterface waterPumpLogic, INotificationService notificationService, IWateringService wateringService)
    {
        _waterPumpLogic = waterPumpLogic;
        _notificationService = notificationService;
        _wateringService = wateringService;
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

    [HttpGet("{id}/water-level")]
    public async Task<ActionResult<int>> GetWaterPumpWaterLevelAsync(int id)
    {
        var waterLevel = await _waterPumpLogic.GetWaterPumpWaterLevelAsync(id);

        if (waterLevel == -1)
        {
            return NotFound($"Water pump with id {id} not found.");
        }

        return Ok(waterLevel);
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
    public async Task<ActionResult<WaterPump>> TriggerManualWateringAsync(int id)
    {
        var pump = await _waterPumpLogic.GetWaterPumpByIdAsync(id);
        if (pump == null)
        {
            return NotFound($"Water pump with id {id} not found.");
        }

        if (pump.WaterLevel < 250)
        {
            Logger.Log(1, $"Water level is low: {pump.WaterLevel}.");

            var refillNotification = new NotificationDTO
            {
                Message = $"Water level is low: {pump.WaterLevel}.",
                Type = "Watering",
                TimeStamp = DateTime.UtcNow
            };
            await _notificationService.SendNotification(refillNotification);

            return BadRequest("Water level is too low to water the plants.");
        }

        //Actual watering, needs the water amount that later converts to ms
        try
        {
            await _wateringService.TriggerWateringAsync(pump.ThresholdValue);
        }
        catch (Exception ex)
        {
            Console.Out.WriteLine(ex.Message);
        }
        // Update the water level in the database, just id needed
        await _waterPumpLogic.TriggerManualWateringAsync(id);

        var waterUsedNotification = new NotificationDTO
        {
            Message = $"Plant watered with {pump.ThresholdValue} ml.",
            Type = "Watering",
            TimeStamp = DateTime.UtcNow
        };

        await _notificationService.SendNotification(waterUsedNotification);

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

        if (updatedPump.WaterLevel > updatedPump.WaterTankCapacity)
        {
            return BadRequest("Water level exceeds tank capacity.");
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

    [HttpPatch("{id}/capacity")]
    public async Task<ActionResult<WaterPump>> UpdateWaterTankCapacityValueAsync(int id, [FromBody] int newCapacityValue)
    {
        var updatedPump = await _waterPumpLogic.UpdateWaterTankCapacityAsync(id, newCapacityValue);
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