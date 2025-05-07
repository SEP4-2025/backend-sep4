using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IPictureInterface
{
    Task<Picture> GetPictureById(int id);
    Task<List<Picture>> GetPictureByPlantIdAsync(int plantId);
    Task<Picture> UpdateNote(int id, string note);
    Task DeletePictureAsync(int id);
}
