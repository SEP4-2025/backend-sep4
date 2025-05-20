using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class LogLogic : ILogInterface
{
    private readonly AppDbContext _context;

    public LogLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Log> AddLogAsync(Log log)
    {
        var newLog = new Log()
        {
            Timestamp = log.Timestamp,
            Message = log.Message,
            GreenhouseId = log.GreenhouseId
        };
        await _context.Logs.AddAsync(newLog);
        await _context.SaveChangesAsync();

        return newLog;
    }

    public async Task<Log> GetLogByIdAsync(int id)
    {
        var log = await _context.Logs.FirstOrDefaultAsync(x => x.Id == id);

        if (log == null)
        {
            throw new Exception("Log not found.");
        }

        return log;
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
        //This method gets water usage for the last 5 days,
        //it is done through logs due to the fact that we do not have a water usage table,
        //and we do not want to add a new table just for this
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-5);

        // Fetch logs for the greenhouse within the date range
        var logs = await _context
            .Logs.Where(x =>
                x.GreenhouseId == greenhouseId && x.Timestamp >= startDate && x.Timestamp < endDate
            )
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

        // Group logs by date
        var dailyData = logs.GroupBy(log => log.Timestamp.Date)
            .Select(group => new { Date = group.Key, Logs = group.ToList() })
            .ToList();

        var results = new List<DailyWaterUsageDTO>();

        // Process each day's logs
        foreach (var day in dailyData)
        {
            // Extract watering logs
            var wateringLogs = day.Logs.Where(l => l.Message.Contains("watered with")).ToList();

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
            var waterLevelLogs = day
                .Logs.Where(l =>
                    l.Message.Contains("Water tank refilled to")
                    || l.Message.Contains("Water level is")
                )
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

            results.Add(
                new DailyWaterUsageDTO
                {
                    Date = day.Date,
                    DailyWaterUsage = dailyWaterUsage,
                    WaterLevel = waterLevel
                }
            );
        }
        return results;
    }

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
