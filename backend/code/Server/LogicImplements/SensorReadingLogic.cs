using Database;
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
        throw new NotImplementedException();
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

    public Task<SensorReading> AddSensorReadingAsync(SensorReading sensorReading)
    {
        throw new NotImplementedException();
    }

    public Task DeleteSensorReadingAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<SensorReading>> GetSensorReadingsAsync()
    {
        return _context.SensorReadings.ToListAsync();
    }
}
