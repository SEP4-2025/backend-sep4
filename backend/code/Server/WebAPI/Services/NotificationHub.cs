using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Services;

public class NotificationHub : Hub
{
    public async Task SendNotification(NotificationDTO notification)
    {
        // Send the notification to all connected clients
        await Clients.All.SendAsync("ReceiveNotification", notification);
    }

    public async Task SendNotificationToUser(string gardenerId, NotificationDTO notification)
    {
        await Clients.User(gardenerId).SendAsync("ReceiveNotification", notification);
    }
}