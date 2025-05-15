using Database;
using DTOs;
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

    public async Task<List<DailyWaterUsageDTO>> GetWaterUsageForLastFiveDaysAsync(int greenhouseId)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-5); 

        // Fetch logs for the greenhouse within the date range
        var logs = await _context.Logs
            .Where(x => x.GreenhouseId == greenhouseId &&
                        x.Timestamp >= startDate &&
                        x.Timestamp < endDate)
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

        // Group logs by date
        var dailyData = logs
            .GroupBy(log => log.Timestamp.Date)
            .Select(group => new
            {
                Date = group.Key,
                Logs = group.ToList()
            })
            .ToList();

        var results = new List<DailyWaterUsageDTO>();

        // Process each day's logs
        foreach (var day in dailyData)
        {
            // Extract watering logs
            var wateringLogs = day.Logs
                .Where(l => l.Message.Contains("watered with"))
                .ToList();

            // Calculate total water usage for the day
            int dailyWaterUsage = 0;
            foreach (var log in wateringLogs)
            {
                // Parse water amount from log messages like "Plant watered with amount: 150."
                var amountText = log.Message.Split("amount:")[1].Trim();
                if (int.TryParse(amountText.Replace(".", ""), out int amount))
                {
                    dailyWaterUsage += amount;
                }
            }

            // Get the latest water level reported for the day
            int waterLevel = 0;
            var waterLevelLogs = day.Logs
                .Where(l => l.Message.Contains("Water tank refilled to") ||
                            l.Message.Contains("Water level is"))
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefault();

            if (waterLevelLogs != null)
            {
                if (waterLevelLogs.Message.Contains("refilled to"))
                {
                    var levelText = waterLevelLogs.Message.Split("refilled to")[1].Trim();
                    int.TryParse(levelText.Replace(".", ""), out waterLevel);
                }
                else if (waterLevelLogs.Message.Contains("Water level is"))
                {
                    var levelText = waterLevelLogs.Message.Split("Water level is")[1].Trim();
                    int.TryParse(levelText.Replace(".", "").Replace(":", ""), out waterLevel);
                }
            }

            results.Add(new DailyWaterUsageDTO
            {
                Date = day.Date,
                DailyWaterUsage = dailyWaterUsage,
                WaterLevel = waterLevel
            });
        }
        return results;
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