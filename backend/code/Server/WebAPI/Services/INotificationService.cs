public interface INotificationService
{
    Task SendNotification(NotificationDTO notification);
    Task SendNotificationToUser(string gardenerId, NotificationDTO notification);
}
