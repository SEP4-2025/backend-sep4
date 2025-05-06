using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IPlantInterface
{
    Task<List<Plant>> GetPlantsAsync();
    Task<Plant> GetPlantByIdAsync(int id);
    Task<Plant> AddPlantAsync(PlantDTO plant);
    Task<Plant> UploadPicture(int id, Picture picture);
    Task<Plant> UpdatePlantNameAsync(int id, string plantName);
    Task DeletePlantAsync(int id);
}
