using DTOs;
using Entities;

namespace LogicInterfaces;

public interface INotificationInterface
{
    Task<List<Notification>> GetNotifications();
    Task<Notification> GetNotificationByType(string type);
}
