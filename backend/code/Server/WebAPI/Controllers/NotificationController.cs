using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//Will turn on authorization later, after initial testing on cloud
// [Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("trigger")]
    public async Task<IActionResult> TriggerNotification(
        [FromBody] NotificationDTO notificationPayload
    )
    {
        try
        {
            await _notificationService.SendNotification(notificationPayload);

            return Ok(new { status = "Notification triggered successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                "An internal error occurred while dispatching the notification."
            );
        }
    }
}
