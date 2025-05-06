using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IPlantInterface
{
    Task<List<Plant>> GetPlantsAsync();
    Task<Plant> GetPlantByIdAsync(int id);
    Task<Plant> AddPlantAsync(PlantDTO plant);

    //Todo Get Plant pictures
    Task<Plant> UpdatePlantNameAsync(int id, string plantName);
    Task DeletePlantAsync(int id);
}
