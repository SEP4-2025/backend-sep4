using Database;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class NotificationLogic : INotificationInterface
{
    private readonly AppDbContext _context;

    public NotificationLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetNotifications()
    {
        return await _context.Notifications.ToListAsync();
    }

    public async Task<Notification> GetNotificationByType(string type)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Type == type);

        if (notification == null)
        {
            throw new KeyNotFoundException($"Notification with type {type} not found.");
        }

        return notification;
    }

    public async Task<Notification> AddNotification(NotificationDTO notification)
    {
        var newNotification = new Notification
        {
            Type = notification.Type,
            Message = notification.Message,
            TimeStamp = DateTime.UtcNow
        };

        _context.Notifications.Add(newNotification);
        await _context.SaveChangesAsync();

        return newNotification;
    }
}
