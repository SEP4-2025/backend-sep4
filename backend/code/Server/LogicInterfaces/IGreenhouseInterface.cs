using Entities;

namespace LogicInterfaces;

public interface IGreenhouseInterface
{
    Task<Greenhouse> GetGreenhouseByIdAsync(int id);
    Task<Greenhouse> GetGreenhouseByNameAsync(string name);
    Task<Greenhouse> GetGreenhouseByGardenerIdAsync(int gardenerId);
    Task AddGreenhouseAsync(Greenhouse greenhouse);
    Task UpdateGreenhouseAsync(Greenhouse greenhouse);
}
