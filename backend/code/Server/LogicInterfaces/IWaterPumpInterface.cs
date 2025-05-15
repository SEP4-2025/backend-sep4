using DTOs;
using Entities;

namespace LogicInterfaces
{
    public interface IWaterPumpInterface
    {
        Task<WaterPump> GetWaterPumpByIdAsync(int id);
        Task<List<WaterPump>> GetAllWaterPumpsAsync();
        Task<int> GetWaterPumpWaterLevelAsync(int id);
        Task<WaterPump> ToggleAutomationStatusAsync(int id, bool autoWatering);
        Task<WaterPump> TriggerManualWateringAsync(int id);
        Task<WaterPump> UpdateCurrentWaterLevelAsync(int id, int addedWaterAmount);
        Task<WaterPump> UpdateThresholdValueAsync(int id, int newThresholdValue);
        Task<WaterPump> UpdateWaterTankCapacityAsync(int id, int newCapacity);
        Task<WaterPump> AddWaterPumpAsync(WaterPumpDTO waterPumpDTO);
        Task DeleteWaterPumpAsync(int id);
    }
}