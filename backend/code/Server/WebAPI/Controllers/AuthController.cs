using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Database;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<Gardener> _passwordHasher;

    public AuthController(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<Gardener>();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        var gardener = await _dbContext.Gardeners.FirstOrDefaultAsync(g =>
            g.Username == loginRequest.Username
        );
        if (gardener == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var isPasswordValid = _passwordHasher.VerifyHashedPassword(
            gardener,
            gardener.Password,
            loginRequest.Password
        );
        if (isPasswordValid == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = GenerateJwtToken(gardener);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(Gardener gardener)
    {
        
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, gardener.Username),
            new Claim(JwtRegisteredClaimNames.NameId, gardener.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDTO changePasswordRequest
    )
    {
        var gardener = await _dbContext.Gardeners.FirstOrDefaultAsync(g =>
            g.Username == changePasswordRequest.Username
        );
        if (gardener == null)
        {
            return NotFound("Gardener not found.");
        }

        var isOldPasswordValid = _passwordHasher.VerifyHashedPassword(
            gardener,
            gardener.Password,
            changePasswordRequest.OldPassword
        );
        if (isOldPasswordValid == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid old password.");
        }
        if (changePasswordRequest.NewPassword != changePasswordRequest.RepeatNewPassword)
        {
            return Unauthorized("New password and repeat password do not match.");
        }

        gardener.Password = _passwordHasher.HashPassword(
            gardener,
            changePasswordRequest.NewPassword
        );
        await _dbContext.SaveChangesAsync();

        return Ok("Password changed successfully.");
    }
}