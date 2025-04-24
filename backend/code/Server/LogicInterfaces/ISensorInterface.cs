using DTOs;
using Entities;

namespace LogicInterfaces;

public interface ISensorInterface
{
    Task<List<Sensor>> GetAllSensorsAsync();
    Task<Sensor> GetSensorByIdAsync(int id);
    Task<Sensor> GetSensorByTypeAsync(string type);
    Task<String> GetMetricUnitBySensorIdAsync(int sensorId);
    Task<List<Sensor>> GetAllSensorsByGreenHouseId(int greenHouseId);
    Task<Sensor> AddSensorAsync(SensorDTO sensor);
    Task<Sensor> UpdateSensorAsync(int id, UpdateSensorDTO sensor);
    Task DeleteSensorAsync(int id);
}
