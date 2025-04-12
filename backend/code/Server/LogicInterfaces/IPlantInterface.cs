using Entities;

namespace LogicInterfaces;

public interface IPlantInterface
{
    Task<Plant> GetPlantByIdAsync(int id);
    Task AddPlantAsync(Plant plant);
    Task UpdatePlantAsync(Plant plant);
    Task DeletePlantAsync(int id);
}
