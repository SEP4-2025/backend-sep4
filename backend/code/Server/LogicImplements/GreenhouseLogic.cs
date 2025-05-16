using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class GreenhouseLogic : IGreenhouseInterface
{
    private readonly AppDbContext _context;

    public GreenhouseLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Greenhouse>> GetGreenhouses()
    {
        return await _context.Greenhouses.ToListAsync();
    }

    public async Task<Greenhouse> GetGreenhouseById(int id)
    {
        var greenhouse = await _context.Greenhouses.FirstOrDefaultAsync(g => g.Id == id);

        if (greenhouse == null)
        {
            throw new KeyNotFoundException($"Greenhouse with id {id} not found.");
        }

        return greenhouse;
    }

    public async Task<Greenhouse> GetGreenhouseByName(string name)
    {
        var gardener = await _context.Greenhouses.FirstOrDefaultAsync(g => g.Name == name);

        if (gardener == null)
        {
            throw new KeyNotFoundException($"Greenhouse with name {name} not found.");
        }

        return gardener;
    }

    public async Task<Greenhouse> GetGreenhouseByGardenerId(int gardenerId)
    {
        var greenhouse = await _context.Greenhouses.FirstOrDefaultAsync(g =>
            g.GardenerId == gardenerId
        );

        if (greenhouse == null)
        {
            throw new KeyNotFoundException($"Greenhouse with gardener id {gardenerId} not found.");
        }

        return greenhouse;
    }

    public async Task<Greenhouse> AddGreenhouse(GreenhouseDTO greenhouse)
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

    public async Task<Greenhouse> UpdateGreenhouseName(int id, string name)
    {
        var greenhouse = await _context.Greenhouses.FirstOrDefaultAsync(g => g.Id == id);

        if (greenhouse == null)
        {
            throw new KeyNotFoundException($"Greenhouse with id {id} not found.");
        }

        greenhouse.Name = name;
        await _context.SaveChangesAsync();

        return greenhouse;
    }

    public async Task<Greenhouse> UpdateGreenhouse(Greenhouse greenhouse)
    {
        var existingGreenhouse = await _context.Greenhouses.FindAsync(greenhouse.Id);

        if (existingGreenhouse == null)
        {
            throw new KeyNotFoundException($"Greenhouse with id {greenhouse.Id} not found.");
        }

        existingGreenhouse.Name = greenhouse.Name;
        existingGreenhouse.GardenerId = greenhouse.GardenerId;

        await _context.SaveChangesAsync();

        return existingGreenhouse;
    }

    public async Task DeleteGreenhouse(int id)
    {
        var greenhouseToDelete = await _context.Greenhouses.FirstOrDefaultAsync(s => s.Id == id);

        if (greenhouseToDelete != null)
        {
            _context.Greenhouses.Remove(greenhouseToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
