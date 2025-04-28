using Entities;

namespace LogicInterfaces;

public interface IPlantInterface
{
    Task<Plant> GetPlantByIdAsync(int id);
    Task<Plant> AddPlantAsync(Plant plant);

    //Todo Get Plant pictures
    Task<Plant> UpdatePlantNameAsync(int id, string plantName);
    Task DeletePlantAsync(int id);
}
