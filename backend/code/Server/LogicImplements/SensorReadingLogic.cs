using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;
using Tools;

namespace LogicImplements;

public class SensorReadingLogic : ISensorReadingInterface
{
    private readonly AppDbContext _context;

    public SensorReadingLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SensorReading> GetSensorReadingByIdAsync(int id)
    {
        var sensorReading = await _context.SensorReadings.FirstOrDefaultAsync(sr => sr.Id == id);

        if (sensorReading == null)
        {
            throw new Exception($"Sensor reading with id {id} not found.");
        }

        return sensorReading;
    }

    public async Task<List<SensorReading>> GetSensorReadingsBySensorIdAsync(int sensorId)
    {
        var sensor = await _context.Sensors.FindAsync(sensorId);
        if (sensor == null)
        {
            return new List<SensorReading>();
        }

        return await _context.SensorReadings.Where(s => s.SensorId == sensorId).ToListAsync();
    }

    public async Task<List<SensorReading>> GetSensorReadingsByDateAsync(
        DateTime start,
        DateTime end
    )
    {
        return await _context
            .SensorReadings.Where(r => r.TimeStamp >= start && r.TimeStamp < end)
            .ToListAsync();
    }

    public async Task<SensorReading> AddSensorReadingAsync(SensorReadingDTO sensorReading)
    {
        var newSensorReading = new SensorReading
        {
            Value = sensorReading.Value,
            TimeStamp = sensorReading.TimeStamp,
            SensorId = sensorReading.SensorId,
        };

        _context.SensorReadings.Add(newSensorReading);
        await _context.SaveChangesAsync();

        Logger.Log(1, $"New sensor reading with id: {newSensorReading.Id} added.");

        return newSensorReading;
    }

    public async Task DeleteSensorReadingAsync(int id)
    {
        var sensorReading = await _context.SensorReadings.FindAsync(id);
        if (sensorReading != null)
        {
            _context.SensorReadings.Remove(sensorReading);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<SensorReading>> GetSensorReadingsAsync()
    {
        return await _context.SensorReadings.ToListAsync();
    }

    public async Task<List<AverageSensorReadingDataDTO>> GetAverageSensorReadingsFromLast24Hours(
        int greenhouseId
    )
    {
        var timeLimit = DateTime.UtcNow.AddHours(-24);

        var result = await _context
            .SensorReadings.Where(sr => sr.TimeStamp >= timeLimit)
            .Join(
                _context.Sensors.Where(s => s.GreenhouseId == greenhouseId),
                sr => sr.SensorId,
                s => s.Id,
                (sr, s) => new { SensorReading = sr, Sensor = s }
            )
            .GroupBy(
                joined => new
                {
                    joined.Sensor.Id,
                    joined.Sensor.Type,
                    joined.Sensor.MetricUnit,
                },
                joined => joined.SensorReading.Value
            )
            .Select(g => new AverageSensorReadingDataDTO
            {
                SensorId = g.Key.Id,
                SensorType = g.Key.Type,
                MetricUnit = g.Key.MetricUnit,
                AverageValue = g.Any() ? g.Average() : null,
            })
            .ToListAsync();

        return result;
    }

    public async Task<List<AverageSensorReadingDataDTO>> GetAverageReadingFromLast7Days(
        int greenhouseId
    )
    {
        var timeLimit = DateTime.UtcNow.AddDays(-7);

        var result = await _context
            .SensorReadings.Where(sr => sr.TimeStamp >= timeLimit)
            .Join(
                _context.Sensors.Where(s => s.GreenhouseId == greenhouseId),
                sr => sr.SensorId,
                s => s.Id,
                (sr, s) => new { SensorReading = sr, Sensor = s }
            )
            .GroupBy(
                joined => new
                {
                    joined.Sensor.Id,
                    joined.Sensor.Type,
                    joined.Sensor.MetricUnit,
                },
                joined => joined.SensorReading.Value
            )
            .Select(g => new AverageSensorReadingDataDTO
            {
                SensorId = g.Key.Id,
                SensorType = g.Key.Type,
                MetricUnit = g.Key.MetricUnit,
                AverageValue = g.Any() ? g.Average() : null,
            })
            .ToListAsync();

        return result;
    }

    public async Task<List<AverageSensorReadingDataDTO>> GetAverageReadingFromLast30Days(
        int greenhouseId
    )
    {
        var timeLimit = DateTime.UtcNow.AddDays(-30);

        var result = await _context
            .SensorReadings.Where(sr => sr.TimeStamp >= timeLimit)
            .Join(
                _context.Sensors.Where(s => s.GreenhouseId == greenhouseId),
                sr => sr.SensorId,
                s => s.Id,
                (sr, s) => new { SensorReading = sr, Sensor = s }
            )
            .GroupBy(
                joined => new
                {
                    joined.Sensor.Id,
                    joined.Sensor.Type,
                    joined.Sensor.MetricUnit,
                },
                joined => joined.SensorReading.Value
            )
            .Select(g => new AverageSensorReadingDataDTO
            {
                SensorId = g.Key.Id,
                SensorType = g.Key.Type,
                MetricUnit = g.Key.MetricUnit,
                AverageValue = g.Any() ? g.Average() : null,
            })
            .ToListAsync();

        return result;
    }
}
