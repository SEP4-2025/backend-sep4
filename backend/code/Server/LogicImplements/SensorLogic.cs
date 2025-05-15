using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;
using Tools;

namespace LogicImplements;

public class SensorLogic : ISensorInterface
{
    private readonly AppDbContext _context;

    public SensorLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sensor>> GetAllSensorsAsync()
    {
        return await _context.Sensors.ToListAsync();
    }

    public async Task<Sensor?> GetSensorByIdAsync(int id)
    {
        return await _context.Sensors.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sensor> AddSensorAsync(AddSensorDTO addSensor)
    {
        var newSensor = new Sensor
        {
            Type = addSensor.Type,
            MetricUnit = addSensor.MetricUnit,
            ThresholdValue = addSensor.ThresholdValue,
            GreenhouseId = addSensor.GreenHouseId,
        };

        _context.Sensors.Add(newSensor);

        await _context.SaveChangesAsync();

        return newSensor;
    }

    public async Task<Sensor> UpdateSensorAsync(int id, UpdateSensorDTO addSensor)
    {
        var existingSensor = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == id);

        if (addSensor.Type is not null)
            existingSensor.Type = addSensor.Type;
        if (addSensor.MetricUnit is not null)
            existingSensor.MetricUnit = addSensor.MetricUnit;
        if (addSensor.ThresholdValue.HasValue)
            existingSensor.ThresholdValue = addSensor.ThresholdValue.Value;

        await _context.SaveChangesAsync();

        return existingSensor;
    }

    public async Task DeleteSensorAsync(int id)
    {
        var sensorToDelete = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == id);
        if (sensorToDelete != null)
        {
            _context.Sensors.Remove(sensorToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
