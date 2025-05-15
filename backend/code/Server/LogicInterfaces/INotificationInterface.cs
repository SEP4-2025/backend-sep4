using Entities;

namespace LogicInterfaces;

public interface INotificationInterface
{
    Task<List<Notification>> GetNotifications();
    Task<Notification> GetNotificationByType(string type);
    Task<Notification> AddNotification(NotificationDTO notification);
}
