using LogicInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

//Will turn on authorization later, after initial testing on cloud
// [Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly INotificationPrefInterface _notificationPrefLogic;
    private readonly INotificationInterface _notificationLogic;

    public NotificationController(
        INotificationService notificationService,
        INotificationPrefInterface notificationPrefLogic,
        INotificationInterface notificationLogic
    )
    {
        _notificationService = notificationService;
        _notificationPrefLogic = notificationPrefLogic;
        _notificationLogic = notificationLogic;
    }

    [HttpPost("trigger")]
    public async Task<IActionResult> TriggerNotification(
        [FromBody] NotificationDTO notificationPayload
    )
    {
        //notification will be added to db even if it does not go through  
        await _notificationLogic.AddNotification(notificationPayload);

        try
        {
            var notificationPrefs = await _notificationPrefLogic.GetNotificationPrefs();
            var gardenerPrefs = notificationPrefs
                .Where(p => p.Type == notificationPayload.Type && p.IsEnabled == true)
                .ToList();
            if (gardenerPrefs.Count == 0)
            {
                return NotFound("Notification disabled");
            }
            else
            {
                await _notificationService.SendNotification(notificationPayload);
            }

            return Ok(new { status = "Notification triggered successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ex + "An internal error occurred while dispatching the notification."
            );
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<NotificationDTO>>> GetNotifications()
    {
        try
        {
            var notifications = await _notificationLogic.GetNotifications();
            if (notifications == null || !notifications.Any())
            {
                return NotFound("No notifications found.");
            }
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex + "An internal error occurred while fetching notifications.");
        }
    }

    [HttpGet("byType")]
    public async Task<ActionResult<NotificationDTO>> GetNotificationByType(string type)
    {
        try
        {
            var notification = await _notificationLogic.GetNotificationByType(type);
            if (notification == null)
            {
                return NotFound($"Notification with type {type} not found.");
            }
            return Ok(notification);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ex + "An internal error occurred while fetching the notification."
            );
        }
    }
}
