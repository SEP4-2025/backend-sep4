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



    public Task<SensorReading> GetSensorReadingByIdAsync(int id)
    {
        return _context.SensorReadings
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<SensorReading> GetSensorReadingBySensorIdAsync(int sensorId)
    {
        return _context.SensorReadings
            .FirstOrDefaultAsync(x => x.SensorId == sensorId);
    }

    public Task<List<SensorReading>> GetSensorReadingsBySensorIdAsync(int sensorId)
    {
        return _context.SensorReadings
            .Where(x => x.SensorId == sensorId)
            .ToListAsync();
    }

    public Task<List<SensorReading>> GetSensorReadingsByDateAsync(DateTime utcDate)
    {
        var start = utcDate.Date;
        var end = utcDate.AddDays(1);
        return _context.SensorReadings
            .Where(sr => sr.TimeStamp >= start && sr.TimeStamp < end)
            .ToListAsync();
    }

    public async Task<SensorReading> AddSensorReadingAsync(SensorReadingDTO sensorReading)
    {
        var entity = new SensorReading
        {
            TimeStamp = sensorReading.TimeStamp,
            Value = sensorReading.Value,
            ThresholdValue = sensorReading.ThresholdValue,
            SensorId = sensorReading.SensorId
        };

        _context.SensorReadings.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Task> DeleteSensorReadingAsync(int id)
    {
        var sensorReading = _context.SensorReadings.Find(id);
        if (sensorReading != null)
        {
            _context.SensorReadings.Remove(sensorReading);
            await _context.SaveChangesAsync();
        }
        return Task.CompletedTask;
    }

    public Task<List<SensorReading>> GetSensorReadingsAsync()
    {
        return _context.SensorReadings.ToListAsync();
    }
}
