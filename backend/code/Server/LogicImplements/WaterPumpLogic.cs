using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;
using Tools;

namespace LogicImplements;

public class WaterPumpLogic : IWaterPumpInterface
{
    private readonly AppDbContext _context;

    public WaterPumpLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WaterPump> GetWaterPumpByIdAsync(int id)
    {
        var waterPump = await _context.WaterPumps.FirstOrDefaultAsync(w => w.Id == id);

        if (waterPump == null)
        {
            throw new KeyNotFoundException($"Water pump with ID {id} not found.");
        }

        return waterPump;
    }

    public async Task<List<WaterPump>> GetAllWaterPumpsAsync()
    {
        return await _context.WaterPumps.ToListAsync();
    }

    public async Task<int> GetWaterPumpWaterLevelAsync(int id)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);

        return waterPump.WaterLevel;
    }

    public async Task<WaterPump> ToggleAutomationStatusAsync(int id, bool autoWatering)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);

        waterPump.AutoWateringEnabled = autoWatering;
        await _context.SaveChangesAsync();

        //hardcoded because we do not handle greenhouseId correctly
        Logger.Log(1, $"Automatic watering changed to {autoWatering}.");

        return waterPump;
    }

    public async Task<WaterPump> TriggerManualWateringAsync(int id)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);

        if (waterPump.WaterLevel < waterPump.ThresholdValue)
        {
            throw new InvalidOperationException("Insufficient water to perform manual watering.");
        }

        waterPump.WaterLevel -= waterPump.ThresholdValue;
        waterPump.LastWateredTime = DateTime.UtcNow;
        waterPump.LastWaterAmount = waterPump.ThresholdValue;

        await _context.SaveChangesAsync();

        //hardcoded because we do not handle greenhouseId correctly
        Logger.Log(1, $"Plant watered with amount: {waterPump.ThresholdValue}.");

        return waterPump;
    }

    public async Task<WaterPump> UpdateCurrentWaterLevelAsync(int id, int addedWaterAmount)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);

        var newAmount = waterPump.WaterLevel += addedWaterAmount;

        //Will not work if the water tank capacity is 0
        if (waterPump.WaterLevel > waterPump.WaterTankCapacity)
        {
            waterPump.WaterLevel = waterPump.WaterTankCapacity;
        }

        //hardcoded because we do not handle greenhouseId correctly
        Logger.Log(1, $"Water tank refilled to {newAmount}.");

        await _context.SaveChangesAsync();
        return waterPump;
    }

    public async Task<WaterPump> UpdateThresholdValueAsync(int id, int newThresholdValue)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);

        waterPump.ThresholdValue = newThresholdValue;
        await _context.SaveChangesAsync();

        return waterPump;
    }

    public async Task<WaterPump> UpdateWaterTankCapacityAsync(int id, int newCapacity)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);

        waterPump.WaterTankCapacity = newCapacity;
        await _context.SaveChangesAsync();

        return waterPump;
    }

    public async Task<WaterPump> AddWaterPumpAsync(WaterPumpDTO waterPumpDto)
    {
        var waterPump = new WaterPump
        {
            Id = waterPumpDto.Id,
            LastWateredTime = waterPumpDto.LastWatered,
            LastWaterAmount = waterPumpDto.LastWaterAmount,
            AutoWateringEnabled = waterPumpDto.AutoWatering,
            WaterTankCapacity = waterPumpDto.WaterTankCapacity,
            WaterLevel = waterPumpDto.CurrentWaterLevel,
            ThresholdValue = waterPumpDto.ThresholdValue
        };

        await _context.WaterPumps.AddAsync(waterPump);
        await _context.SaveChangesAsync();
        return waterPump;
    }

    public async Task DeleteWaterPumpAsync(int id)
    {
        var pump = await _context.WaterPumps.FirstOrDefaultAsync(p => p.Id == id);
        if (pump != null)
        {
            _context.WaterPumps.Remove(pump);
            await _context.SaveChangesAsync();
        }
    }
}
