using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class PictureController : ControllerBase
{
    private readonly IPictureInterface _pictureInterface;

    public PictureController(IPictureInterface pictureInterface)
    {
        _pictureInterface = pictureInterface;
    }
    
    [HttpPost]
    public async Task<ActionResult<Picture>> AddPicture([FromQuery] PictureDTO picture)
    {
        if (picture.IsEmpty()) return BadRequest("Picture is null");
        var newPicture = await _pictureInterface.AddPictureAsync(picture);
        return CreatedAtAction(nameof(GetPicturesByPlantId), new { plantId = newPicture.PlantId }, newPicture);
    }

    [HttpGet("{plantId}")]
    public async Task<ActionResult<List<Picture>>> GetPicturesByPlantId(int plantId)
    {
        var pictures = await _pictureInterface.GetPictureByPlantIdAsync(plantId);
        return Ok(pictures);
    }

    [HttpPut]
    public async Task<ActionResult<Picture>> UpdatePictureNote(int id, string note)
    {
        var picture = _pictureInterface.GetPictureByPlantIdAsync(id);
        if (picture == null) return NotFound($"Picture with id {id} not found");
        await _pictureInterface.UpdateNote(id, note);
        return Ok(picture);
    }

    [HttpDelete("{Id}")]
    public async Task<ActionResult> DeletePicture(int Id)
    {
        var picture = await _pictureInterface.GetPictureByPlantIdAsync(Id);
        if (picture == null) return NotFound($"Picture with id {Id} not found");
        await _pictureInterface.DeletePictureAsync(Id);
        return NoContent();
    }
}