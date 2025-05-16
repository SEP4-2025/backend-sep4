using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class PictureLogic : IPictureInterface
{
    private readonly AppDbContext _context;

    public PictureLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Picture> GetPictureById(int id)
    {
        var picture = await _context.Pictures.FirstOrDefaultAsync(p => p.Id == id);

        if (picture == null)
        {
            throw new Exception("Picture not found.");
        }

        return picture;
    }

    public async Task<List<Picture>> GetPictureByPlantIdAsync(int plantId)
    {
        return await _context.Pictures.Where(p => p.PlantId == plantId).ToListAsync();
    }

    public async Task<Picture> AddPictureAsync(PictureDTO pictureDto)
    {
        if (pictureDto == null)
        {
            throw new Exception("Picture data is invalid.");
        }

        var uploader = new GCSUploader.GCSUploader();
        var fileName = Guid.NewGuid().ToString();
        var url = await uploader.UploadImageAsync(pictureDto.File, fileName);

        var picture = new Picture()
        {
            PlantId = pictureDto.PlantId,
            TimeStamp = DateTime.UtcNow,
            Url = url,
        };

        if (pictureDto.Note is not null)
        {
            picture.Note = pictureDto.Note;
        }

        await _context.Pictures.AddAsync(picture);
        await _context.SaveChangesAsync();

        return picture;
    }

    public async Task<Picture> UpdateNote(int id, string note)
    {
        var picture = await _context.Pictures.FirstOrDefaultAsync(p => p.Id == id);

        if (picture == null)
        {
            throw new Exception($"Sensor with ID {id} not found.");
        }
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
