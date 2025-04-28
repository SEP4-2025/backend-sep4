using DTOs;
using Entities;

namespace LogicInterfaces
{
    public interface IWaterPumpInterface
    {
        Task<WaterPump> GetWaterPumpByIdAsync(int id);
        Task<List<WaterPump>> GetAllWaterPumpsAsync();
        Task<WaterPump> UpdateAutoWateringStatusAsync(int id, bool autoWatering);
        Task<WaterPump> TriggerManualWateringAsync(int id, int waterAmount);
        Task<WaterPump> UpdateCurrentWaterLevelAsync(int id, int addedWaterAmount);
        Task<WaterPump> UpdateThresholdValueAsync(int id, int newThresholdValue);
        Task<WaterPump> AddWaterPumpAsync(WaterPumpDTO waterPumpDTO);
        Task DeleteWaterPumpAsync(int id);
    }
}