using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class NotificationPrefLogic : INotificationPrefInterface
{
    private readonly AppDbContext _context;

    public NotificationPrefLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationPreferences>> GetNotificationPrefs()
    {
        return await _context.NotificationPreferences.ToListAsync();
    }

    public async Task<List<NotificationPreferences>> GetNotificationPrefsByGardenerId(
        int gardenerId
    )
    {
        var preferences = await _context
            .NotificationPreferences.Where(p => p.GardenerId == gardenerId)
            .ToListAsync();

        if (preferences == null || preferences.Count == 0)
        {
            throw new KeyNotFoundException(
                $"No notification preferences found for gardener {gardenerId}."
            );
        }

        return preferences;
    }

    public async Task UpdateNotificationPref(int gardenerId, string type)
    {
        var preference = await _context
            .NotificationPreferences.Where(p => p.GardenerId == gardenerId && p.Type == type)
            .FirstOrDefaultAsync();

        if (preference == null)
        {
            throw new KeyNotFoundException(
                $"Notification preference for gardener {gardenerId} and type {type} not found."
            );
        }

        preference.IsEnabled = !preference.IsEnabled;
        await _context.SaveChangesAsync();
    }
}
