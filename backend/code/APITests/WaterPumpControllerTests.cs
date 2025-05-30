﻿using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ReceiverService;
using Tools;
using WebAPI.Controllers;
using WebAPI.Services;

namespace APITests
{
    [TestFixture]
    public class WaterPumpControllerTests
    {
        private Mock<IWaterPumpInterface> _mockWaterPumpLogic;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IWateringService> _mockMqttWateringService;
        private WaterPumpController _controller;
        private WaterPump _testPump;
        private WaterPumpDTO _testPumpDto;

        [SetUp]
        public void Setup()
        {
            _mockWaterPumpLogic = new Mock<IWaterPumpInterface>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockMqttWateringService = new Mock<IWateringService>();
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb").EnableSensitiveDataLogging()
            );
            services.AddScoped<DbContext, AppDbContext>();
            var provider = services.BuildServiceProvider();
            var _context = provider.GetRequiredService<AppDbContext>();
            Logger.Initialize(_context);
            _controller = new WaterPumpController(
                _mockWaterPumpLogic.Object,
                _mockNotificationService.Object,
                _mockMqttWateringService.Object
            );
            _testPump = new WaterPump
            {
                Id = 1,
                ThresholdValue = 200,
                WaterLevel = 500,
                WaterTankCapacity = 1000,
                AutoWateringEnabled = true,
            };
            _testPumpDto = new WaterPumpDTO
            {
                CurrentWaterLevel = 500,
                ThresholdValue = 200,
                AutoWatering = true
            };
        }

        [Test]
        public async Task GetWaterPumpByIdAsync_ReturnsOk_WhenPumpExists()
        {
            _mockWaterPumpLogic.Setup(x => x.GetWaterPumpByIdAsync(1)).ReturnsAsync(_testPump);
            var result = await _controller.GetWaterPumpByIdAsync(1);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(_testPump));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task GetWaterPumpByIdAsync_ReturnsNotFound_WhenPumpDoesNotExist()
        {
            _mockWaterPumpLogic
                .Setup(x => x.GetWaterPumpByIdAsync(1))
                .ReturnsAsync((WaterPump)null!);
            var result = await _controller.GetWaterPumpByIdAsync(1);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAllWaterPumpsAsync_ReturnsOk_WithAllPumps()
        {
            var pumps = new List<WaterPump> { _testPump };
            _mockWaterPumpLogic.Setup(x => x.GetAllWaterPumpsAsync()).ReturnsAsync(pumps);
            var result = await _controller.GetAllWaterPumpsAsync();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(pumps));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task AddWaterPumpAsync_ReturnsBadRequest_WhenDtoIsNull()
        {
            var result = await _controller.AddWaterPumpAsync(null!);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddWaterPumpAsync_ReturnsOk_WhenDtoIsValid()
        {
            _mockWaterPumpLogic
                .Setup(x => x.AddWaterPumpAsync(_testPumpDto))
                .ReturnsAsync(_testPump);
            var result = await _controller.AddWaterPumpAsync(_testPumpDto);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(_testPump));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task UpdateAutoWateringStatusAsync_ReturnsOk_WhenPumpExists()
        {
            _mockWaterPumpLogic
                .Setup(x => x.ToggleAutomationStatusAsync(1, true))
                .ReturnsAsync(_testPump);
            var result = await _controller.ToggleAutomationStatusAsync(1, true);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(_testPump));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task UpdateAutoWateringStatusAsync_ReturnsNotFound_WhenPumpDoesNotExist()
        {
            _mockWaterPumpLogic
                .Setup(x => x.ToggleAutomationStatusAsync(1, true))
                .ReturnsAsync((WaterPump)null!);
            var result = await _controller.ToggleAutomationStatusAsync(1, true);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task TriggerManualWateringAsync_ReturnsOk_WhenPumpExists()
        {
            _mockWaterPumpLogic.Setup(x => x.GetWaterPumpByIdAsync(1)).ReturnsAsync(_testPump);
            _mockWaterPumpLogic.Setup(x => x.TriggerManualWateringAsync(1)).ReturnsAsync(_testPump);
            var result = await _controller.TriggerManualWateringAsync(1);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(_testPump));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task TriggerManualWateringAsync_ReturnsNotFound_WhenPumpDoesNotExist()
        {
            _mockWaterPumpLogic
                .Setup(x => x.GetWaterPumpByIdAsync(1))
                .ReturnsAsync((WaterPump)null!);
            var result = await _controller.TriggerManualWateringAsync(1);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateCurrentWaterLevelAsync_ReturnsOk_WhenPumpExists()
        {
            _mockWaterPumpLogic
                .Setup(x => x.UpdateCurrentWaterLevelAsync(1, 100))
                .ReturnsAsync(_testPump);
            var result = await _controller.UpdateCurrentWaterLevelAsync(1, 100);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(_testPump));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task UpdateCurrentWaterLevelAsync_ReturnsNotFound_WhenPumpDoesNotExist()
        {
            _mockWaterPumpLogic
                .Setup(x => x.UpdateCurrentWaterLevelAsync(1, 100))
                .ReturnsAsync((WaterPump)null!);
            var result = await _controller.UpdateCurrentWaterLevelAsync(1, 100);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateThresholdValueAsync_ReturnsOk_WhenPumpExists()
        {
            _mockWaterPumpLogic
                .Setup(x => x.UpdateThresholdValueAsync(1, 150))
                .ReturnsAsync(_testPump);
            var result = await _controller.UpdateThresholdValueAsync(1, 150);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(_testPump));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task UpdateThresholdValueAsync_ReturnsNotFound_WhenPumpDoesNotExist()
        {
            _mockWaterPumpLogic
                .Setup(x => x.UpdateThresholdValueAsync(1, 150))
                .ReturnsAsync((WaterPump)null!);
            var result = await _controller.UpdateThresholdValueAsync(1, 150);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteWaterPumpAsync_ReturnsNoContent_WhenPumpExists()
        {
            _mockWaterPumpLogic.Setup(x => x.GetWaterPumpByIdAsync(1)).ReturnsAsync(_testPump);
            _mockWaterPumpLogic.Setup(x => x.DeleteWaterPumpAsync(1)).Returns(Task.CompletedTask);
            var result = await _controller.DeleteWaterPumpAsync(1);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteWaterPumpAsync_ReturnsNotFound_WhenPumpDoesNotExist()
        {
            _mockWaterPumpLogic
                .Setup(x => x.GetWaterPumpByIdAsync(1))
                .ReturnsAsync((WaterPump)null!);
            var result = await _controller.DeleteWaterPumpAsync(1);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task UpdateWaterTankCapacityAsync_ReturnsOk_WhenPumpExists()
        {
            _mockWaterPumpLogic
                .Setup(x => x.UpdateWaterTankCapacityAsync(1, 1000))
                .ReturnsAsync(_testPump);
            var result = await _controller.UpdateWaterTankCapacityValueAsync(1, 1000);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(_testPump));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }
    }
}
