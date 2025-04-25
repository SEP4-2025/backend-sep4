using DTOs;
using Entities;

namespace LogicInterfaces;

public interface ISensorInterface
{
    Task<List<Sensor>> GetAllSensorsAsync();
    Task<Sensor> GetSensorByIdAsync(int id);
    Task<Sensor> AddSensorAsync(SensorDTO sensor);
    Task<Sensor> UpdateSensorAsync(Sensor sensor);
    Task DeleteSensorAsync(int id);
}
