using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SensorReadingController : ControllerBase
{
    private readonly ISensorReadingInterface sensorReading;

    public SensorReadingController(ISensorReadingInterface sensorReading)
    {
        this.sensorReading = sensorReading;
    }

    [HttpGet]
    public async Task<ActionResult<SensorReading>> GetSensorReadings()
    {
        var readings = await sensorReading.GetSensorReadingsAsync();
        if (readings == null || !readings.Any())
        {
            return NotFound("No sensor readings found.");
        }
        return Ok(readings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SensorReading>> GetSensorReadingById(int id)
    {
        var reading = await sensorReading.GetSensorReadingByIdAsync(id);
        if (reading == null)
        {
            return NotFound($"Sensor reading with ID {id} not found.");
        }
        return Ok(reading);
    }

    [HttpGet("sensor/{sensorId}")]
    public async Task<ActionResult<List<SensorReading>>> GetSensorReadingsBySensorId(int sensorId)
    {
        var readings = await sensorReading.GetSensorReadingsBySensorIdAsync(sensorId);
        if (readings == null || readings.Count == 0)
        {
            return NotFound($"No readings found for sensor with ID {sensorId}.");
        }
        return Ok(readings);
    }

    [HttpGet("date/{date}")]
    public async Task<ActionResult<List<SensorReading>>> GetSensorReadingsByDate(
        [FromRoute] DateTime date
    )
    {
        var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        var readings = await sensorReading.GetSensorReadingsByDateAsync(utcDate);
        if (readings == null || readings.Count == 0)
        {
            return NotFound($"No readings found on {utcDate.ToShortDateString()}.");
        }
        return Ok(readings);
    }

    [HttpPost]
    public async Task<ActionResult<SensorReading>> AddSensorReading(
        [FromBody] SensorReadingDTO sensorReading
    )
    {
        if (sensorReading == null)
        {
            return BadRequest("Sensor reading cannot be null.");
        }

        var addedSensorReading = await this.sensorReading.AddSensorReadingAsync(sensorReading);
        return CreatedAtAction(
            nameof(GetSensorReadingById),
            new { id = addedSensorReading.Id },
            addedSensorReading
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSensorReading(int id)
    {
        var reading = await sensorReading.GetSensorReadingByIdAsync(id);
        if (reading == null)
        {
            return NotFound($"Sensor reading with ID {id} not found.");
        }

        await sensorReading.DeleteSensorReadingAsync(id);
        return NoContent();
    }
}
