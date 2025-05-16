using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly ILogInterface _logInterface;

    public LogController(ILogInterface logInterface)
    {
        _logInterface = logInterface;
    }

    [HttpGet("{id}/water-usage")]
    public async Task<ActionResult<List<DailyWaterUsageDTO>>> GetWaterUsageForLastFiveDays(int id)
    {
        var waterUsage = await _logInterface.GetWaterUsageForLastFiveDaysAsync(id);
        if (waterUsage == null || !waterUsage.Any())
        {
            return NotFound($"No water usage data found for greenhouse with id {id}");
        }
        return Ok(waterUsage);
    }

    [HttpGet]
    public async Task<ActionResult<List<Log>>> GetLogs()
    {
        return await _logInterface.GetAllLogs();
    }

    [HttpGet("date/{date}")]
    public async Task<ActionResult<List<Log>>> GetLogsByDate(DateTime date)
    {
        var logs = await _logInterface.GetLogsByDateAsync(date);
        if (logs is null)
            return NotFound($"No logs found for date {date}");
        return logs;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLog(int id)
    {
        var log = await _logInterface.GetLogByIdAsync(id);
        if (log == null)
            return NotFound($"Log with id {id} not found");
        await _logInterface.DeleteLogAsync(id);
        return NoContent();
    }
}
