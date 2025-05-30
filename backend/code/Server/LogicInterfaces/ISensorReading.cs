using DTOs;
using Entities;

namespace LogicInterfaces;

public interface ISensorReadingInterface
{
    Task<SensorReading> GetSensorReadingByIdAsync(int id);
    Task<List<SensorReading>> GetSensorReadingsBySensorIdAsync(int sensorId);
    Task<List<SensorReading>> GetSensorReadingsByDateAsync(DateTime start, DateTime end);
    Task<SensorReading> AddSensorReadingAsync(SensorReadingDTO sensorReading);
    Task DeleteSensorReadingAsync(int id);
    Task<List<SensorReading>> GetSensorReadingsAsync();
    Task<List<AverageSensorReadingDataDTO>> GetAverageSensorReadingsFromLast24Hours(int greenhouseId);
    Task<List<AverageSensorReadingDataDTO>> GetAverageReadingFromLast7Days(int greenhouseId);
    Task<List<AverageSensorReadingDataDTO>> GetAverageReadingFromLast30Days(int greenhouseId);
}
