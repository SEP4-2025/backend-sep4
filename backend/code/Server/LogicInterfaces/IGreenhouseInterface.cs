using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IGreenhouseInterface
{
    Task<Greenhouse> GetGreenhouseByIdAsync(int id);
    Task<Greenhouse> GetGreenhouseByNameAsync(string name);
    Task<Greenhouse> GetGreenhouseByGardenerIdAsync(int gardenerId);
    Task<Greenhouse> AddGreenhouseAsync(GreenhouseDTO greenhouse);
    Task<Greenhouse> UpdateGreenhouseName(int id, string name);
    Task DeleteGreenhouseAsync(int id);
}
