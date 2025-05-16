using DTOs;
using Entities;

namespace LogicInterfaces;

public interface ILogInterface
{
    Task<Log> AddLogAsync(Log log);
    Task<Log> GetLogByIdAsync(int id);
    Task<List<Log>> GetLogsByDateAsync(DateTime date);
    Task<List<Log>> GetAllLogs();
    Task<List<DailyWaterUsageDTO>> GetWaterUsageForLastFiveDaysAsync(int greenhouseId);
    Task DeleteLogAsync(int id);
}
