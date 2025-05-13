using DTOs;
using Entities;

namespace LogicInterfaces;

public interface INotificationPrefInterface
{
    Task<List<NotificationPreferences>> GetNotificationPrefs();
    Task UpdateNotificationPref(int gardenerId, string type);
}
