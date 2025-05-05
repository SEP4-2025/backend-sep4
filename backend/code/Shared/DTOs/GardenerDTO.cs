namespace DTOs;

public class GardenerDTO
{
    public string? Username { get; set; }
    public string? Password { get; set; }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password);
    }
    
    public bool ValueMissing()
    {
        return string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Password);
    }

}