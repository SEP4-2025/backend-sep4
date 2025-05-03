using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IGardenerInterface
{
    Task<List<Gardener>> GetGardeners();
    Task<Gardener> GetGardenerByIdAsync(int id);
    Task<Gardener> AddGardenerAsync(GardenerDTO addGardener);
    Task<Gardener> UpdateGardenerAsync(int id, GardenerDTO gardener);
    Task DeleteGardenerAsync(int id);
}
