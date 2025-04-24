using Database;
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

    public async Task<Sensor> GetSensorByGreenhouseIdAsync(int greenhouseId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Sensor>> GetSensorsByGreenhouseIdAsync(int greenhouseId)
    {
        var sensors = await _context.Sensors.Where(s => s.GreenhouseId == greenhouseId).ToListAsync();
        return sensors;
    }

    public async Task<List<Sensor>> GetSensorsByTypeAsync(string type)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Sensor>> GetAllSensorsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Sensor> UpdateSensorThresholdAsync(int id, double threshold)
    {
        throw new NotImplementedException();
    }

    public async Task<Sensor> UpdateSensorMetricUnitAsync(int id, string metricUnit)
    {
        throw new NotImplementedException();
    }

    public async Task AddSensorAsync(Sensor sensor)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateSensorAsync(Sensor sensor)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteSensorAsync(int id)
    {
        var sensor = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == id);
        if (sensor == null)
        {
            throw new Exception($"Sensor with id {id} not found");
        }
        _context.Sensors.Remove(sensor);
        await _context.SaveChangesAsync();
    }
}