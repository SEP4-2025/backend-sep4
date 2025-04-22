using Entities;
using LogicInterfaces;

namespace LogicImplements;

public class SensorReadingLogic : ISensorReadingInterface
{
    private readonly List<SensorReading> sensorReadings;

    // Add a constructor to initialize the list
    public SensorReadingLogic()
    {
        sensorReadings = new List<SensorReading>();
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
        sensorReading.Id = sensorReadings.Any() ? sensorReadings.Max(sr => sr.Id) + 1 : 1;
        sensorReadings.Add(sensorReading);
        return Task.FromResult(sensorReading);
    }

    public Task DeleteSensorReadingAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<SensorReading>> GetSensorReadingsAsync()
    {
        return Task.FromResult(sensorReadings);
    }
}
