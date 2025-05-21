using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IPlantInterface
{
    Task<List<Plant>> GetPlantsAsync();
    Task<Plant> GetPlantByIdAsync(int id);
    Task<Plant> AddPlantAsync(PlantDTO plant);
    Task<Plant> UpdatePlantNameAsync(int id, string plantName);
    Task<Plant> UpdatePlantSpeciesAsync(int id, string species);
    Task DeletePlantAsync(int id);
}
