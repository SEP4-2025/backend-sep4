using Microsoft.AspNetCore.Http;

namespace DTOs;

public class PictureDTO
{
    public IFormFile File { get; set; } = null!;
    public string? Note { get; set; }
    public int PlantId { get; set; }

    public bool IsEmpty()
    {
        return File == null || File.Length == 0 || PlantId == 0;
    }
}