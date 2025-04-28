using Database;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class LogLogic : ILogInterface
{
    public readonly AppDbContext _context;

    public LogLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Log?> GetLogByIdAsync(int id)
    {
        return await _context.Logs.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Log>> GetLogsByDateAsync(DateTime date)
    {
        return await _context.Logs.Where(x => x.Timestamp == date).ToListAsync();
    }

    public async Task<List<Log>> GetAllLogs()
    {
        return await _context.Logs.ToListAsync();
    }

    // public async Task<List<Log>> GetLogsBySensorIdAsync(int sensorId)
    // {
    //     return await _context.Logs.Where(x => x.Id == sensorId).ToListAsync();
    // }

    // public async Task<List<Log>> GetLogsByWaterPumpIdAsync(int pumpId)
    // {
    //     return await _context.Logs.Where(x => x.Id == pumpId).ToListAsync();
    // }

    // public async Task<Log> AddLogAsync(Log log)
    // {
    //     var newLog = new Log()
    //     {
    //         Timestamp = log.Timestamp,
    //         Message = log.Message,
    //         SensorReadingId = log.SensorReadingId,
    //         WaterPumpId = log.WaterPumpId,
    //         GreenhouseId = log.GreenhouseId
    //     };
    //     await _context.Logs.AddAsync(newLog);
    //     await _context.SaveChangesAsync();
    //     
    //     return newLog;
    // }

    public async Task DeleteLogAsync(int id)
    {
        var log = await _context.Logs.FirstOrDefaultAsync(x => x.Id == id);
        if (log != null)
        {
            _context.Logs.Remove(log);
            await _context.SaveChangesAsync();
        }
    }
}