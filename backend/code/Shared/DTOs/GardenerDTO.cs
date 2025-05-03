namespace DTOs;

public class GardenerDTO
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    
    public bool IsEmpty()
    {
        return Username == null || Password == null;
    }
}