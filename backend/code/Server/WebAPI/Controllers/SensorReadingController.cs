using Microsoft.AspNetCore.Mvc;
using Entities;
using LogicInterfaces;

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
        return Ok(readings);
    }

    [HttpPost]
    public async Task<ActionResult<SensorReading>> AddSensorReading([FromBody] SensorReading sensorReading)
    {
        if (sensorReading == null)
        {
            return BadRequest("Sensor reading cannot be null");
        }

        var addedSensorReading = await this.sensorReading.AddSensorReadingAsync(sensorReading);
        return CreatedAtAction(nameof(GetSensorReadings), new { id = addedSensorReading.Id }, addedSensorReading);
    }
}