using Microsoft.AspNetCore.SignalR;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotification(NotificationDTO notification)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }

    public async Task SendNotificationToUser(string userId, NotificationDTO notification)
    {
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
    }
}
