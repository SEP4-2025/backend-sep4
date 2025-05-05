using System.ComponentModel.DataAnnotations;

public class ChangePasswordRequestDTO
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string OldPassword { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    public string RepeatNewPassword { get; set; }
}
