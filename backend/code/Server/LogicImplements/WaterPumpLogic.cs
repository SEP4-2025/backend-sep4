using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;
using Tools;

namespace LogicImplements;

public class WaterPumpLogic : IWaterPumpInterface
{
    public AppDbContext _context;

    public WaterPumpLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WaterPump?> GetWaterPumpByIdAsync(int id)
    {
        return await _context.WaterPumps.FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<List<WaterPump>> GetAllWaterPumpsAsync()
    {
        return await _context.WaterPumps.ToListAsync();
    }
    
    public async Task<int> GetWaterPumpWaterLevelAsync(int id)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);
        if (waterPump == null) return -1;

        return waterPump.WaterLevel;
    }

    public async Task<WaterPump> ToggleAutomationStatusAsync(int id, bool autoWatering)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);
        if (waterPump == null) return null;

        waterPump.AutoWateringEnabled = autoWatering;
        await _context.SaveChangesAsync();

        //hardcoded because we do not handle greenhouseId correctly
        Logger.Log(1, $"Automatic watering changed to {autoWatering}.");

        return waterPump;
    }

    public async Task<WaterPump> TriggerManualWateringAsync(int id, int waterAmount)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);
        if (waterPump.WaterLevel < waterAmount) return null;

        waterPump.WaterLevel -= waterAmount;
        waterPump.LastWateredTime = DateTime.UtcNow;
        waterPump.LastWaterAmount = waterAmount;

        await _context.SaveChangesAsync();

        //hardcoded because we do not handle greenhouseId correctly
        Logger.Log(1, $"Manually watered with amount: {waterAmount}.");
        
        return waterPump;
    }

    public async Task<WaterPump> UpdateCurrentWaterLevelAsync(int id, int addedWaterAmount)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);
        if (waterPump == null) return null;

        waterPump.WaterLevel += addedWaterAmount;
        
        //Will not work if the water tank capacity is 0 
        if (waterPump.WaterLevel > waterPump.WaterTankCapacity)
        {
            waterPump.WaterLevel = waterPump.WaterTankCapacity;
        }

        await _context.SaveChangesAsync();
        return waterPump;
    }

    public async Task<WaterPump> UpdateThresholdValueAsync(int id, int newThresholdValue)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);
        if (waterPump == null) return null;

        waterPump.ThresholdValue = newThresholdValue;
        await _context.SaveChangesAsync();

        return waterPump;
    }

    public async Task<WaterPump> UpdateWaterTankCapacityAsync(int id, int newCapacity)
    {
        var waterPump = await GetWaterPumpByIdAsync(id);
        if (waterPump == null) return null;

        waterPump.WaterTankCapacity = newCapacity;
        await _context.SaveChangesAsync();

        return waterPump;
    }

    public async Task<WaterPump> AddWaterPumpAsync(WaterPumpDTO waterPumpDTO)
    {
        var waterPump = new WaterPump
        {
            Id = waterPumpDTO.Id,
            LastWateredTime = waterPumpDTO.LastWatered,
            LastWaterAmount = waterPumpDTO.LastWaterAmount,
            AutoWateringEnabled = waterPumpDTO.AutoWatering,
            WaterTankCapacity = waterPumpDTO.WaterTankCapacity,
            WaterLevel = waterPumpDTO.CurrentWaterLevel,
            ThresholdValue = waterPumpDTO.ThresholdValue
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