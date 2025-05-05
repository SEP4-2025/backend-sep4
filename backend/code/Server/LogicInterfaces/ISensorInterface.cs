using DTOs;
using Entities;

namespace LogicInterfaces;

public interface ISensorInterface
{
    Task<List<Sensor>> GetAllSensorsAsync();
    Task<Sensor> GetSensorByIdAsync(int id);
    Task<Sensor> AddSensorAsync(AddSensorDTO addSensor);
    Task<Sensor> UpdateSensorAsync(int id, UpdateSensorDTO addSensor);
    Task DeleteSensorAsync(int id);
}
