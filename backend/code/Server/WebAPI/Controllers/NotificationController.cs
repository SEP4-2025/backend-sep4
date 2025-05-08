using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        if (notificationPayload == null)
        {
            return BadRequest("Notification payload cannot be null.");
        }

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
