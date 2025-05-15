using DTOs;
using Entities;

namespace LogicInterfaces;

public interface INotificationPrefInterface
{
    Task<List<NotificationPreferences>> GetNotificationPrefs();

    Task<List<NotificationPreferences>> GetNotificationPrefsByGardenerId(int gardenerId);
    Task UpdateNotificationPref(int gardenerId, string type);
}
