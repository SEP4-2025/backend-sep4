using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class SensorController : ControllerBase
{
    private readonly ISensorInterface sensor;

    public SensorController(ISensorInterface sensor)
    {
        this.sensor = sensor;
    }

    [HttpGet]
    public async Task<ActionResult<Sensor>> GetSensors()
    {
        var sensors = await sensor.GetAllSensorsAsync();
        if (sensors == null || !sensors.Any())
        {
            return NotFound("No sensors found.");
        }
        return Ok(sensors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sensor>> GetSensorById(int id)
    {
        var sensorById = await sensor.GetSensorByIdAsync(id);
        if (sensorById == null)
        {
            return NotFound($"Sensor with ID {id} not found.");
        }
        return Ok(sensorById);
    }

    [HttpPost]
    public async Task<ActionResult<Sensor>> AddSensor([FromBody] AddSensorDTO addSensorDto)
    {
        if (addSensorDto.IsEmpty())
        {
            return BadRequest($"Sensor data is required.");
        }

        var addedSensor = await sensor.AddSensorAsync(addSensorDto);
        return CreatedAtAction(nameof(GetSensorById), new { id = addedSensor.Id }, addedSensor);
    }

    [HttpPatch("update/{id}")]
    public async Task<ActionResult<Sensor>> UpdateSensor(int id, [FromBody] UpdateSensorDTO sensorToUpdate)
    {
        if (sensorToUpdate.IsEmpty())
        {
            return BadRequest("Sensor data is required.");
        }
        var updatedSensor = await sensor.UpdateSensorAsync(id, sensorToUpdate);
        if (updatedSensor == null)
        {
            return NotFound($"Sensor with ID {id} not found.");
        }

        return Ok(updatedSensor);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSensor(int id)
    {
        var existing = await sensor.GetSensorByIdAsync(id);
        if (existing == null)
        {
            return NotFound($"Sensor with ID {id} not found.");
        }

        await sensor.DeleteSensorAsync(id);
        return Ok("Sensor deleted successfully.");
    }

}