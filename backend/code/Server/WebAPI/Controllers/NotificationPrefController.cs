using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationPrefController : ControllerBase
{
    private readonly INotificationPrefInterface _notificationPrefLogic;

    public NotificationPrefController(INotificationPrefInterface notificationPrefLogic)
    {
        _notificationPrefLogic = notificationPrefLogic;
    }

    [HttpGet]
    public async Task<ActionResult<NotificationPreferences>> GetNotificationPrefs()
    {
        var notificationPrefs = await _notificationPrefLogic.GetNotificationPrefs();
        if (notificationPrefs == null || !notificationPrefs.Any())
        {
            return NotFound("No notification preferences found.");
        }
        return Ok(notificationPrefs);
    }

    [HttpPatch("toggle")]
    public async Task<ActionResult> UpdateNotificationPref(
        [FromBody] NotificationToggleDto toggleDto
    )
    {
        if (toggleDto == null)
        {
            return BadRequest("Invalid notification preference data.");
        }

        try
        {
            await _notificationPrefLogic.UpdateNotificationPref(
                toggleDto.GardenerId,
                toggleDto.Type
            );
            return Ok("Notification preference updated successfully.");
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound("Notification preference not found.");
        }
    }

    [HttpGet("{gardenerId}")]
    public async Task<ActionResult<NotificationPreferences>> GetNotificationPrefsByGardenerId(
        int gardenerId
    )
    {
        var notificationPrefs = await _notificationPrefLogic.GetNotificationPrefsByGardenerId(
            gardenerId
        );
        if (notificationPrefs == null || !notificationPrefs.Any())
        {
            return NotFound($"No notification preferences found for gardener {gardenerId}.");
        }
        return Ok(notificationPrefs);
    }
}
