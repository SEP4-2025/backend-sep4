using System.ComponentModel.DataAnnotations;

public class LoginRequestDTO
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
