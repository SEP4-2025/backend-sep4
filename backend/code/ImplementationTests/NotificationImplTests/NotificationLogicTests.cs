using System.Collections.Generic;
using System.Threading.Tasks;
using Database;
using Entities;
using LogicImplements;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ImplementationTests.NotificationImplTests;

public class NotificationLogicTests
{
    private AppDbContext _context;
    private INotificationInterface _notificationLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _notificationLogic = new NotificationLogic(_context);
    }

    [Test]
    public async Task GetNotifications_ReturnsAllNotifications()
    {
        _context.Notifications.Add(
            new Notification
            {
                Type = "info",
                Message = "Test",
                TimeStamp = System.DateTime.UtcNow
            }
        );
        await _context.SaveChangesAsync();

        var result = await _notificationLogic.GetNotifications();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);
    }

    [Test]
    public async Task GetNotificationByType_ReturnsNotification()
    {
        var notification = new Notification
        {
            Type = "alert",
            Message = "Alert!",
            TimeStamp = System.DateTime.UtcNow
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var result = await _notificationLogic.GetNotificationByType("alert");

        Assert.IsNotNull(result);
        Assert.That(result.Type, Is.EqualTo("alert"));
    }

    [Test]
    public Task GetNotificationByType_Throws_WhenNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _notificationLogic.GetNotificationByType("notfound")
        );
        Assert.That(ex.Message, Is.EqualTo("Notification with type notfound not found."));
        return Task.CompletedTask;
    }

    [Test]
    public async Task AddNotification_AddsNotification()
    {
        var dto = new NotificationDTO { Type = "new", Message = "New notification" };

        var result = await _notificationLogic.AddNotification(dto);

        Assert.IsNotNull(result);
        Assert.That(result.Type, Is.EqualTo("new"));
        Assert.That(result.Message, Is.EqualTo("New notification"));
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.Notifications.RemoveRange(_context.Notifications);
        await _context.SaveChangesAsync();
    }
}
