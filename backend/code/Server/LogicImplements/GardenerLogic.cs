using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class GardenerLogic : IGardenerInterface
{
    private readonly AppDbContext _context;

    public GardenerLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Gardener>> GetGardeners()
    {
        return await _context.Gardeners.ToListAsync();
    }

    public async Task<Gardener?> GetGardenerByIdAsync(int id)
    {
        return await _context.Gardeners.FindAsync(id);
    }

    public async Task<Gardener> AddGardenerAsync(GardenerDTO gardener)
    {
        var newGardener = new Gardener
        {
            Username = gardener.Username,
            Password = gardener.Password,
        };

        _context.Gardeners.Add(newGardener);
        await _context.SaveChangesAsync();

        return newGardener;
    }

    public async Task<Gardener> UpdateGardenerAsync(Gardener gardener)
    {
        var existingGardener = await _context.Gardeners.FindAsync(gardener.Id);

        existingGardener.Username = gardener.Username;
        existingGardener.Password = gardener.Password;

        await _context.SaveChangesAsync();
        return existingGardener;
    }

    public async Task DeleteGardenerAsync(int id)
    {
        var existingGardener = await _context.Gardeners.FindAsync(id);
        if (existingGardener != null)
        {
            _context.Gardeners.Remove(existingGardener);
            await _context.SaveChangesAsync();
        }
    }
}