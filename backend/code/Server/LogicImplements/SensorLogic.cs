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
    
    public async Task<Sensor> GetSensorByTypeAsync(string type)
    {
        var sensor = await _context.Sensors.FirstOrDefaultAsync(s => s.Type == type);
        if (sensor == null)
        {
            throw new Exception($"Sensor type {type} not found");
        }
        return sensor;
    }

    public async Task<String> GetMetricUnitBySensorIdAsync(int sensorId)
    {
        var sensor = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == sensorId);
        if (sensor == null)
        {
            throw new Exception($"Sensor with id {sensorId} not found");
        }
        return sensor.MetricUnit;
    }

    public async Task<List<Sensor>> GetAllSensorsByGreenHouseId(int greenHouseId)
    {
        var sensors = await _context.Sensors.Where(s => s.GreenhouseId == greenHouseId).ToListAsync();
        return sensors;
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
    
    public async Task<Sensor> UpdateSensorAsync(int id, UpdateSensorDTO sensor)
    {
        var sensorToUpdate = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == id);
        if (sensorToUpdate == null)
        {
            throw new Exception($"Sensor with id {id} not found");
        }
        sensorToUpdate.Type = sensor.Type ?? sensorToUpdate.Type;
        sensorToUpdate.MetricUnit = sensor.MetricUnit ?? sensorToUpdate.MetricUnit;
        
        _context.Sensors.Update(sensorToUpdate);
        await _context.SaveChangesAsync();
        return sensorToUpdate;
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