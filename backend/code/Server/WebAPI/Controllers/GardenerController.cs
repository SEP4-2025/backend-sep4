using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;


[ApiController]
[Route("[controller]")]

public class GardenerController : ControllerBase
{
    private readonly IGardenerInterface _gardener;

    public GardenerController(IGardenerInterface gardenerInterface)
    {
        _gardener = gardenerInterface;
    }

    [HttpGet]
    public async Task<ActionResult<List<Gardener>>> GetGardeners()
    {
        var gardeners = await _gardener.GetGardeners();
        if (gardeners == null || !gardeners.Any())
        {
            return NotFound("No gardeners found.");
        }
        return Ok(gardeners);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Gardener>> GetGardenerById(int id)
    {
        var gardener = await _gardener.GetGardenerByIdAsync(id);
        if (gardener == null)
        {
            return NotFound($"Gardener with ID {id} not found.");
        }
        return Ok(gardener);
    }

    [HttpPost]
    public async Task<ActionResult<Gardener>> AddGardenerAsync([FromBody] GardenerDTO gardener)
    {
        if (gardener == null)
        {
            return BadRequest("Gardener data is required.");
        }

        var addedGardener = await _gardener.AddGardenerAsync(gardener);
        return CreatedAtAction(nameof(GetGardenerById), new { id = addedGardener.Id }, addedGardener);
    }

    [HttpPatch]
    public async Task<ActionResult<Gardener>> UpdateGardener([FromBody] Gardener gardener)
    {
        if (gardener == null)
        {
            return BadRequest("Gardener data is required.");
        }

        var gardenerToUpdate = await _gardener.GetGardenerByIdAsync(gardener.Id);
        if (gardenerToUpdate == null)
        {
            return NotFound($"Gardener with ID {gardener.Id} not found.");
        }
        await _gardener.UpdateGardenerAsync(gardener);
        return Ok(gardenerToUpdate);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Gardener>> DeleteGardener(int id)
    {
        var gardener = await _gardener.GetGardenerByIdAsync(id);
        if (gardener == null)
        {
            return NotFound($"Gardener with ID {id} not found.");
        }
        try
        {
            await _gardener.DeleteGardenerAsync(id);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("Greenhouse_gardenerid_fkey") == true)
            {
                return BadRequest("Cannot delete Gardener because it is associated with a Greenhouse.");
            }
            throw;
        }
        return NoContent();
    }

}