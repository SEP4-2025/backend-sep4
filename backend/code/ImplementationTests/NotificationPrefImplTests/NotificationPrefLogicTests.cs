using System.Collections.Generic;
using System.Threading.Tasks;
using Database;
using Entities;
using LogicImplements;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ImplementationTests.NotificationPrefImplTests;

public class NotificationPrefLogicTests
{
    private AppDbContext _context;
    private INotificationPrefInterface _notificationPrefLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _notificationPrefLogic = new NotificationPrefLogic(_context);
    }

    [Test]
    public async Task GetNotificationPrefs_ReturnsAllPrefs()
    {
        _context.NotificationPreferences.Add(
            new NotificationPreferences
            {
                GardenerId = 1,
                Type = "info",
                IsEnabled = true
            }
        );
        await _context.SaveChangesAsync();
        var result = await _notificationPrefLogic.GetNotificationPrefs();
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);
    }

    [Test]
    public async Task GetNotificationPrefsByGardenerId_ReturnsPrefs()
    {
        var pref = new NotificationPreferences
        {
            GardenerId = 2,
            Type = "alert",
            IsEnabled = true
        };
        _context.NotificationPreferences.Add(pref);
        await _context.SaveChangesAsync();
        var result = await _notificationPrefLogic.GetNotificationPrefsByGardenerId(2);
        Assert.IsNotNull(result);
        Assert.That(result[0].GardenerId, Is.EqualTo(2));
    }

    [Test]
    public Task GetNotificationPrefsByGardenerId_Throws_WhenNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _notificationPrefLogic.GetNotificationPrefsByGardenerId(-1)
        );
        Assert.That(ex.Message, Is.EqualTo("No notification preferences found for gardener -1."));
        return Task.CompletedTask;
    }

    [Test]
    public async Task UpdateNotificationPref_TogglesIsEnabled()
    {
        var pref = new NotificationPreferences
        {
            GardenerId = 3,
            Type = "toggle",
            IsEnabled = false
        };
        _context.NotificationPreferences.Add(pref);
        await _context.SaveChangesAsync();
        await _notificationPrefLogic.UpdateNotificationPref(3, "toggle");
        var updated = await _context.NotificationPreferences.FirstAsync(p =>
            p.GardenerId == 3 && p.Type == "toggle"
        );
        Assert.IsTrue(updated.IsEnabled);
    }

    [Test]
    public Task UpdateNotificationPref_Throws_WhenNotFound()
    {
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _notificationPrefLogic.UpdateNotificationPref(-1, "none")
        );
        Assert.That(
            ex.Message,
            Is.EqualTo("Notification preference for gardener -1 and type none not found.")
        );
        return Task.CompletedTask;
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.NotificationPreferences.RemoveRange(_context.NotificationPreferences);
        await _context.SaveChangesAsync();
    }
}
