using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IGreenhouseInterface
{
    Task<List<Greenhouse>> GetGreenhouses();
    Task<Greenhouse> GetGreenhouseById(int id);
    Task<Greenhouse> GetGreenhouseByName(string name);
    Task<Greenhouse> GetGreenhouseByGardenerId(int gardenerId);
    Task<Greenhouse> AddGreenhouse(GreenhouseDTO greenhouse);
    Task<Greenhouse> UpdateGreenhouseName(int id, string name);
    Task<Greenhouse> UpdateGreenhouse(Greenhouse greenhouse);
    Task DeleteGreenhouse(int id);
}
