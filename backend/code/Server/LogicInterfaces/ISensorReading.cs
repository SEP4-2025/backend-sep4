using DTOs;
using Entities;

namespace LogicInterfaces;

public interface ISensorReadingInterface
{
    Task<SensorReading> GetSensorReadingByIdAsync(int id);
    Task<List<SensorReading>> GetSensorReadingsBySensorIdAsync(int sensorId);
    Task<List<SensorReading>> GetSensorReadingsByDateAsync(DateTime date);
    Task<SensorReading> AddSensorReadingAsync(SensorReadingDTO sensorReading);
    Task DeleteSensorReadingAsync(int id);
    Task<List<SensorReading>> GetSensorReadingsAsync();
    Task<List<SensorReadingDataDTO>> GetAverageSensorReadingsFromLast24Hours(int greenhouseId);
    Task<List<SensorReadingDataDTO>> GetAverageReadingFromLast7Days(int greenhouseId);
    Task<List<SensorReadingDataDTO>> GetAverageReadingFromLast30Days(int greenhouseId);
}
