using LogicInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//Will turn on authorization later, after initial testing on cloud
// [Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly INotificationPrefInterface _notificationPrefLogic;

    public NotificationController(
        INotificationService notificationService,
        INotificationPrefInterface notificationPrefLogic
    )
    {
        _notificationService = notificationService;
        _notificationPrefLogic = notificationPrefLogic;
    }

    [HttpPost("trigger")]
    public async Task<IActionResult> TriggerNotification(
        [FromBody] NotificationDTO notificationPayload
    )
    {
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
}
