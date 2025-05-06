namespace DTOs;

public class PictureDTO
{
    public string Url { get; set; }
    public string? Note { get; set; }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Note) ;
    }
}