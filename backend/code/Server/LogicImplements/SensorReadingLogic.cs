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

    public async Task<SensorReading> GetSensorReadingByIdAsync(int id)
    {
        return await _context.SensorReadings
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }
    
    public Task<SensorReading> GetSensorReadingBySensorIdAsync(int sensorId)
    {
        throw new NotImplementedException();
    }

    public Task<List<SensorReading>> GetSensorReadingsBySensorIdAsync(int sensorId)
    {
        throw new NotImplementedException();
    }

    public Task<List<SensorReading>> GetSensorReadingsByDateAsync(DateTime date)
    {
        throw new NotImplementedException();
    }

    public async Task<SensorReading> AddSensorReadingAsync(SensorReadingDTO sensorReading)
    {
        var newSensorReading = new SensorReading
        {
            Value = sensorReading.Value,
            TimeStamp = sensorReading.TimeStamp,
            ThresholdValue = sensorReading.ThresholdValue,
            SensorId = sensorReading.SensorId,
        };

        _context.SensorReadings.Add(newSensorReading);  
        await _context.SaveChangesAsync();  

        return newSensorReading; 
    }


    public Task<Task> DeleteSensorReadingAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<SensorReading>> GetSensorReadingsAsync()
    {
        return await _context.SensorReadings.ToListAsync();
    }
}
