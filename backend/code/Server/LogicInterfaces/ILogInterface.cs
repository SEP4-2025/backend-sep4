using Entities;

namespace LogicInterfaces;

public interface ILogInterface
{
    Task<Log> GetLogByIdAsync(int id);
    Task<List<Log>> GetLogsByDateAsync(DateTime date);
    Task<List<Log>> GetAllLogs();
    // Task<List<Log>> GetLogsBySensorIdAsync(int sensorId);
    // Task<List<Log>> GetLogsByWaterPumpIdAsync(int pumpId);
    // Task<Log> AddLogAsync(Log log);
    Task DeleteLogAsync(int id);
}
