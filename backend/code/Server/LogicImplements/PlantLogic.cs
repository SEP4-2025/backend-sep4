using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class PlantLogic : IPlantInterface
{
    public AppDbContext _context;

    public PlantLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Plant>> GetPlantsAsync()
    {
        return await _context.Plants.ToListAsync();
    }
    public async Task<Plant?> GetPlantByIdAsync(int id)
    {
        return await _context.Plants.FirstOrDefaultAsync(p => p.Id == id);
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

    public Task<Plant> UploadPlantPicture(int id, Picture file)
    {
        throw new NotImplementedException();
    }

    public async Task<Plant> UpdatePlantNameAsync(int id, string plantName)
    {
        var existingPlant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == id);

        existingPlant.Name = plantName;

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