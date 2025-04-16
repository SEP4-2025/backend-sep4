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
}