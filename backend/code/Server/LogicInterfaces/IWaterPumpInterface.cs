using Entities;

namespace LogicInterfaces;

public interface IWaterPumpInterface
{
    Task<WaterPump> GetWaterPumpByIdAsync(int id);
    Task<WaterPump> GetRecentUsageDataAsync(int waterPumpId);
    Task<WaterPump> GetCurrentWaterLevelAsync(int waterPumpId);
    Task<WaterPump> UpdateAutoWateringStatusAsync(int waterPumpId, bool status);
    Task<WaterPump> UpdateWaterTankCapacityAsync(int waterPumpId, int capacity);
    Task<WaterPump> UpdateThresholdValueAsync(int waterPumpId, int newThreshold);
    Task AddWaterPumpAsync(WaterPump waterPump);
    Task UpdateWaterPumpAsync(WaterPump waterPump);
}
