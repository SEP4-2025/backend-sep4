using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebAPI.Controllers;
using WebAPI.Services;

namespace APITests
{
    [TestFixture]
    public class NotificationControllerTests
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<INotificationPrefInterface> _notificationPrefLogicMock;
        private Mock<INotificationInterface> _notificationLogicMock;
        private NotificationController _controller;

        [SetUp]
        public void Setup()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _notificationPrefLogicMock = new Mock<INotificationPrefInterface>();
            _notificationLogicMock = new Mock<INotificationInterface>();
            _controller = new NotificationController(
                _notificationServiceMock.Object,
                _notificationPrefLogicMock.Object,
                _notificationLogicMock.Object
            );
        }

        [Test]
        public async Task TriggerNotification_ReturnsOk_WhenNoPrefsEnabled()
        {
            var notification = new NotificationDTO { Type = "type", Message = "msg" };
            var entity = new Notification { Id = 1 };
            _notificationLogicMock.Setup(x => x.AddNotification(notification)).ReturnsAsync(entity);
            _notificationPrefLogicMock
                .Setup(x => x.GetNotificationPrefs())
                .ReturnsAsync(new List<NotificationPreferences>());
            var result = await _controller.TriggerNotification(notification);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task TriggerNotification_ReturnsOk_WhenPrefsEnabled()
        {
            var notification = new NotificationDTO { Type = "type", Message = "msg" };
            var entity = new Notification { Id = 2 };
            var prefs = new List<NotificationPreferences>
            {
                new NotificationPreferences { Type = "type", IsEnabled = true }
            };
            _notificationLogicMock.Setup(x => x.AddNotification(notification)).ReturnsAsync(entity);
            _notificationPrefLogicMock.Setup(x => x.GetNotificationPrefs()).ReturnsAsync(prefs);
            _notificationServiceMock
                .Setup(x => x.SendNotification(notification))
                .Returns(Task.CompletedTask);
            var result = await _controller.TriggerNotification(notification);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetNotifications_ReturnsOk_WhenNotificationsExist()
        {
            var notifications = new List<Notification>
            {
                new Notification { Type = "type", Message = "msg" }
            };
            _notificationLogicMock.Setup(x => x.GetNotifications()).ReturnsAsync(notifications);
            var result = await _controller.GetNotifications();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetNotifications_ReturnsNotFound_WhenNoNotifications()
        {
            _notificationLogicMock
                .Setup(x => x.GetNotifications())
                .ReturnsAsync(new List<Notification>());
            var result = await _controller.GetNotifications();
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetNotifications_Returns500_OnException()
        {
            _notificationLogicMock
                .Setup(x => x.GetNotifications())
                .ThrowsAsync(new System.Exception());
            var result = await _controller.GetNotifications();
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        }
    }
}
