using Entities;

namespace LogicInterfaces;

public interface IPlantInterface
{
    Task<Plant> GetPlantByIdAsync(int id);
    Task<Plant> AddPlantAsync(Plant plant);
    Task<Plant> UpdatePlantNameAsync(int id, string plantName);
    Task DeletePlantAsync(int id);
}
