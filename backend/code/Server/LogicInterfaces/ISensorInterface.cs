using Entities;

namespace LogicInterfaces;

public interface ISensorInterface
{
    Task<Sensor> GetSensorByIdAsync(int id);
    Task<Sensor> GetSensorByTypeAsync(string type);
    Task<Sensor> GetSensorByGreenhouseIdAsync(int greenhouseId);
    Task<List<Sensor>> GetSensorsByGreenhouseIdAsync(int greenhouseId);
    Task<List<Sensor>> GetSensorsByTypeAsync(string type);
    Task<List<Sensor>> GetAllSensorsAsync();
    Task<Sensor> UpdateSensorThresholdAsync(int id, double threshold);
    Task<Sensor> UpdateSensorMetricUnitAsync(int id, string metricUnit);
    Task AddSensorAsync(Sensor sensor);
    Task UpdateSensorAsync(Sensor sensor);
    Task DeleteSensorAsync(int id);
}
