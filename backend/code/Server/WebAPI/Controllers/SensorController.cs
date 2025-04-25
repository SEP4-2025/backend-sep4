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
        return Ok(sensors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sensor>> GetSensorByIdAsync(int id)
    {
        var sensorById = await sensor.GetSensorByIdAsync(id);
        return Ok(sensorById);
    }

    [HttpPost]
    public async Task<ActionResult<Sensor>> AddSensor([FromBody] SensorDTO sensor)
    {
        if (sensor == null)
        {
            return BadRequest("Sensor cannot be null");
        }

        var addedSensor = await this.sensor.AddSensorAsync(sensor);
        return CreatedAtAction(nameof(GetSensors), new { id = addedSensor.Id }, addedSensor);
    }

    [HttpPatch]
    public async Task<ActionResult<Sensor>> UpdateSensor([FromBody] Sensor sensor)
    {
        var sensorToUpdate = await this.sensor.UpdateSensorAsync(sensor);
        return Ok(sensorToUpdate);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteSensor(int id)
    {
        var sensorToDelete = await sensor.GetSensorByIdAsync(id);
        if (sensorToDelete == null)
        {
            return BadRequest("Sensor does not exist");
        }
        await sensor.DeleteSensorAsync(sensorToDelete.Id);
        return NoContent();
    }
}