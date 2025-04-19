using Entities;

namespace LogicInterfaces;

public interface ISensorReadingInterface
{
    Task<SensorReading> GetSensorReadingByIdAsync(int id);
    Task<SensorReading> GetSensorReadingBySensorIdAsync(int sensorId);
    Task<List<SensorReading>> GetSensorReadingsBySensorIdAsync(int sensorId);
    Task<List<SensorReading>> GetSensorReadingsByDateAsync(DateTime date);
    Task<SensorReading> AddSensorReadingAsync(SensorReading sensorReading);
    Task DeleteSensorReadingAsync(int id);
    Task<List<SensorReading>> GetSensorReadingsAsync();
}
