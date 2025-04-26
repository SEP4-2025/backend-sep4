using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class GreenhouseLogic : IGreenhouseInterface
{
    public readonly AppDbContext _context;

    public GreenhouseLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Greenhouse?> GetGreenhouseByIdAsync(int id)
    {
        return await _context.Greenhouses.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Greenhouse?> GetGreenhouseByNameAsync(string name)
    {
        return await _context.Greenhouses.FirstOrDefaultAsync(g => g.Name == name);
    }

    public async Task<Greenhouse?> GetGreenhouseByGardenerIdAsync(int gardenerId)
    {
        return await _context.Greenhouses.FirstOrDefaultAsync(g => g.GardenerId == gardenerId);
    }

    public async Task<Greenhouse> AddGreenhouseAsync(GreenhouseDTO greenhouse)
    {
        var newGreenhouse = new Greenhouse()
        {
            Name = greenhouse.Name,
            GardenerId = greenhouse.GardenerId
        };

        _context.Greenhouses.Add(newGreenhouse);
        await _context.SaveChangesAsync();

        return newGreenhouse;

    }

    public async Task<Greenhouse> UpdateGreenhouseAsync(Greenhouse greenhouse)
    {
        var existingGreenhouse = await _context.Greenhouses.FindAsync(greenhouse.Id);

        existingGreenhouse.Name = greenhouse.Name;
        existingGreenhouse.GardenerId = greenhouse.GardenerId;

        await _context.SaveChangesAsync();
        return existingGreenhouse;
    }

    public async Task DeleteGreenhouseAsync(int id)
    {
        var greenhouseToDelete = await _context.Greenhouses.FirstOrDefaultAsync(s => s.Id == id);
        if (greenhouseToDelete != null)
        {

            _context.Greenhouses.Remove(greenhouseToDelete);
            await _context.SaveChangesAsync();
        }
    }
}