using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class SensorReadingLogic : ISensorReadingInterface
{
    private readonly AppDbContext _context;

    public SensorReadingLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SensorReading?> GetSensorReadingByIdAsync(int id)
    {
        return await _context.SensorReadings.FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public async Task<List<SensorReading>> GetSensorReadingsBySensorIdAsync(int sensorId)
    {
        var sensor = await _context.Sensors.FindAsync(sensorId);
        if (sensor == null)
        {
            return new List<SensorReading>();
        }

        return await _context.SensorReadings.Where(s => s.SensorId == sensorId).ToListAsync();
    }

    public async Task<List<SensorReading>> GetSensorReadingsByDateAsync(DateTime date)
    {
        return await _context.SensorReadings.Where(sr => sr.TimeStamp == date).ToListAsync();
    }

    public async Task<SensorReading> AddSensorReadingAsync(SensorReadingDTO sensorReading)
    {
        var newSensorReading = new SensorReading
        {
            Value = sensorReading.Value,
            TimeStamp = sensorReading.TimeStamp,
            SensorId = sensorReading.SensorId,
        };

        _context.SensorReadings.Add(newSensorReading);
        await _context.SaveChangesAsync();

        return newSensorReading;
    }


    public async Task DeleteSensorReadingAsync(int id)
    {
        var sensorReading = await _context.SensorReadings.FindAsync(id);
        if (sensorReading != null)
        {
            _context.SensorReadings.Remove(sensorReading);
            await _context.SaveChangesAsync();
        }
    }


    public async Task<List<SensorReading>> GetSensorReadingsAsync()
    {
        return await _context.SensorReadings.ToListAsync();
    }
}
