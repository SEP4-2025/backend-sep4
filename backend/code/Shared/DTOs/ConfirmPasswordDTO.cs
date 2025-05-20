using System.ComponentModel.DataAnnotations;

public class ConfirmPasswordDTO
{
    [Required]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
