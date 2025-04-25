using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class SensorLogic : ISensorInterface
{
    private readonly AppDbContext _context;

    public SensorLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sensor>> GetAllSensorsAsync()
    {
        return await _context.Sensors.ToListAsync();
    }

    public async Task<Sensor> GetSensorByIdAsync(int id)
    {
        var sensor = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == id);
        if (sensor == null)
        {
            throw new Exception($"Sensor with id {id} not found");
        }

        return sensor;
    }

    public async Task<Sensor> AddSensorAsync(SensorDTO sensor)
    {
        var newSensor = new Sensor
        {
            Type = sensor.Type,
            MetricUnit = sensor.MetricUnit,
            GreenhouseId = sensor.greenHouseId
        };

        _context.Sensors.Add(newSensor);
        await _context.SaveChangesAsync();

        return newSensor;
    }

    public async Task<Sensor> UpdateSensorAsync(Sensor sensor)
    {
        var existingSensor = await _context.Sensors.FindAsync(sensor.Id);
        if (existingSensor == null)
        {
            throw new Exception($"Sensor with ID {sensor.Id} not found.");
        }

        existingSensor.Type = sensor.Type;
        existingSensor.MetricUnit = sensor.MetricUnit;
        existingSensor.GreenhouseId = sensor.GreenhouseId;

        await _context.SaveChangesAsync();

        return existingSensor;
    }

    public async Task DeleteSensorAsync(int id)
    {
        var sensorToDelete = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == id);
        if (sensorToDelete == null)
        {
            throw new Exception($"Sensor with id {id} not found");
        }
        _context.Sensors.Remove(sensorToDelete);
        await _context.SaveChangesAsync();
    }
}