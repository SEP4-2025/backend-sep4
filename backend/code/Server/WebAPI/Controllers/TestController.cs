using DTOs;
using Entities;

namespace WebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using Database;

[ApiController]
[Route("[controller]")]

public class TestController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("ping-db")]
    public IActionResult PingDb()
    {
        try
        {
            var count = _context.Gardeners.Count();
            return Ok($"Success. Gardeners in DB: {count}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateGardener([FromBody] CreateGardenerDto dto)
    {
        var gardener = new Gardener
        {
            Username = dto.Username,
            Password = dto.Password
        };

        _context.Gardeners.Add(gardener);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGardener), new { id = gardener.Id }, gardener);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGardener(int id)
    {
        var gardener = await _context.Gardeners.FindAsync(id);
        if (gardener == null)
            return NotFound();

        return Ok(gardener);
    }

}