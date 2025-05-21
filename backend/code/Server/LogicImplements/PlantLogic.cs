using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tools;

namespace LogicImplements;

public class PlantLogic : IPlantInterface
{
    private readonly AppDbContext _context;

    public PlantLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Plant>> GetPlantsAsync()
    {
        return await _context.Plants.ToListAsync();
    }

    public async Task<Plant> GetPlantByIdAsync(int id)
    {
        var plant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == id);

        if (plant == null)
        {
            throw new Exception("Plant not found.");
        }

        return plant;
    }

    public async Task<Plant> AddPlantAsync(PlantDTO plant)
    {
        var newPlant = new Plant()
        {
            Name = plant.Name,
            Species = plant.Species,
            Pictures = new List<Picture>(),
            GreenhouseId = plant.GreenhouseId,
        };

        await _context.Plants.AddAsync(newPlant);
        await _context.SaveChangesAsync();
        return newPlant;
    }

    public async Task<Plant> UpdatePlantNameAsync(int id, string plantName)
    {
        var existingPlant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == id);

        if (existingPlant == null)
        {
            throw new Exception("Plant not found.");
        }
        
        existingPlant.Name = plantName;

        await _context.SaveChangesAsync();

        return existingPlant;
    }

    public async Task<Plant> UpdatePlantSpeciesAsync(int id, string species)
    {
        var existingPlant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == id);

        if (existingPlant == null)
        {
            throw new Exception("Plant not found.");
        }

        existingPlant.Species = species;

        await _context.SaveChangesAsync();

        return existingPlant;
    }

    public async Task DeletePlantAsync(int id)
    {
        var plant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == id);

        if (plant != null)
        {
            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();
        }
    }
}
