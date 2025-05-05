using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class PictureLogic : IPictureInterface
{
    public readonly AppDbContext _context;

    public PictureLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Picture?> GetPictureById(int id)
    {
        return await _context.Pictures.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Picture>> GetPictureByPlantIdAsync(int plantId)
    {
        return await _context.Pictures.Where(p => p.PlantId == plantId).ToListAsync();
    }

    public async Task<Picture> AddPictureAsync(PictureDTO picture)
    {
        var newPicture = new Picture()
        {
            Url = picture.Url,
            Note = picture.Note,
            TimeStamp = DateTime.UtcNow,
            PlantId = picture.PlantId
        };
        _context.Pictures.Add(newPicture);
        await _context.SaveChangesAsync();
        return newPicture;
    }

    public async Task<Picture> UpdateNote(int id, string note)
    {
        var picture = await _context.Pictures.FirstOrDefaultAsync(p => p.Id == id);
        if (picture == null) throw new Exception($"Sensor with ID {picture.Id} not found.");
        picture.Note = note;

        await _context.SaveChangesAsync();
        return picture;
    }

    public async Task DeletePictureAsync(int id)
    {
        var picture = await _context.Pictures.FirstOrDefaultAsync(p => p.Id == id);
        if (picture != null)
        {
            _context.Pictures.Remove(picture);
            await _context.SaveChangesAsync();
        }
    }
}