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

    public async Task<Gardener> AddGardenerAsync(GardenerDTO addGardener)
    {
        var newGardener = new Gardener
        {
            Username = addGardener.Username,
            Password = addGardener.Password,
        };

        _context.Gardeners.Add(newGardener);
        await _context.SaveChangesAsync();

        return newGardener;
    }

    public async Task<Gardener> UpdateGardenerAsync(int id, GardenerDTO gardener)
    {
        var existingGardener = await _context.Gardeners.FirstOrDefaultAsync(g => g.Id == id);

        if (gardener.Username is not null)
            existingGardener.Username = gardener.Username;
        if (gardener.Password is not null)
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
