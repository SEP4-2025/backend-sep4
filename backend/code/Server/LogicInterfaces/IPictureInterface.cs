using Entities;

namespace LogicInterfaces;

public interface IPictureInterface
{
    Task<Picture> GetPictureByIdAsync(int id);
    Task<List<Picture>> GetPicturesByDateAsync(DateTime date);
    Task<List<Picture>> GetAllPictures();
    Task<Picture> AddPictureNoteAsync(int id, string note);
    Task AddPictureAsync(Picture picture);
    Task DeletePictureAsync(int id);
}
