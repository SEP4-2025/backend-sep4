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

namespace APITests
{
    [TestFixture]
    public class NotificationPrefControllerTests
    {
        private Mock<INotificationPrefInterface> _notificationPrefLogicMock;
        private NotificationPrefController _controller;

        [SetUp]
        public void Setup()
        {
            _notificationPrefLogicMock = new Mock<INotificationPrefInterface>();
            _controller = new NotificationPrefController(_notificationPrefLogicMock.Object);
        }

        [Test]
        public async Task GetNotificationPrefs_ReturnsOk_WhenPrefsExist()
        {
            var prefs = new List<NotificationPreferences>
            {
                new NotificationPreferences { Type = "type", IsEnabled = true }
            };
            _notificationPrefLogicMock.Setup(x => x.GetNotificationPrefs()).ReturnsAsync(prefs);
            var result = await _controller.GetNotificationPrefs();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetNotificationPrefs_ReturnsNotFound_WhenNoPrefs()
        {
            _notificationPrefLogicMock
                .Setup(x => x.GetNotificationPrefs())
                .ReturnsAsync(new List<NotificationPreferences>());
            var result = await _controller.GetNotificationPrefs();
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateNotificationPref_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.UpdateNotificationPref(null!);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateNotificationPref_ReturnsOk_WhenSuccess()
        {
            var dto = new NotificationToggleDto { GardenerId = 1, Type = "type" };
            _notificationPrefLogicMock
                .Setup(x => x.UpdateNotificationPref(dto.GardenerId, dto.Type))
                .Returns(Task.CompletedTask);
            var result = await _controller.UpdateNotificationPref(dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task UpdateNotificationPref_ReturnsNotFound_OnConcurrencyException()
        {
            var dto = new NotificationToggleDto { GardenerId = 1, Type = "type" };
            _notificationPrefLogicMock
                .Setup(x => x.UpdateNotificationPref(dto.GardenerId, dto.Type))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException());
            var result = await _controller.UpdateNotificationPref(dto);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetNotificationPrefsByGardenerId_ReturnsOk_WhenPrefsExist()
        {
            var prefs = new List<NotificationPreferences>
            {
                new NotificationPreferences { Type = "type", IsEnabled = true }
            };
            _notificationPrefLogicMock
                .Setup(x => x.GetNotificationPrefsByGardenerId(1))
                .ReturnsAsync(prefs);
            var result = await _controller.GetNotificationPrefsByGardenerId(1);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetNotificationPrefsByGardenerId_ReturnsNotFound_WhenNoPrefs()
        {
            _notificationPrefLogicMock
                .Setup(x => x.GetNotificationPrefsByGardenerId(1))
                .ReturnsAsync(new List<NotificationPreferences>());
            var result = await _controller.GetNotificationPrefsByGardenerId(1);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}
