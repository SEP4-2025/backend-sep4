using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IGardenerInterface
{
    Task<List<Gardener>> GetGardeners();
    Task<Gardener> GetGardenerByIdAsync(int id);
    Task<Gardener> AddGardenerAsync(GardenerDTO gardener);
    Task<Gardener> UpdateGardenerAsync(Gardener gardener);
    Task DeleteGardenerAsync(int id);
}
