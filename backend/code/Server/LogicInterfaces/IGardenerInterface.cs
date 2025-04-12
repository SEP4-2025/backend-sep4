using Entities;

namespace LogicInterfaces;

public interface IGardenerInterface
{
    Task<Gardener> GetGardenerByIdAsync(int id);
    Task AddGardenerAsync(Gardener gardener);
    Task UpdateGardenerAsync(Gardener gardener);
}
