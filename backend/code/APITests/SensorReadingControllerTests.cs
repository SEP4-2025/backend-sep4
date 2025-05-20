using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class SensorReadingControllerTests
    {
        private Mock<ISensorReadingInterface> _mockSensorReading;
        private SensorReadingController _controller;

        [SetUp]
        public void Setup()
        {
            _mockSensorReading = new Mock<ISensorReadingInterface>();
            _controller = new SensorReadingController(_mockSensorReading.Object);
        }

        [Test]
        public async Task GetSensorReadings_ReturnsOk_WhenReadingsExist()
        {
            var readings = new List<SensorReading>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            };
            _mockSensorReading.Setup(x => x.GetSensorReadingsAsync()).ReturnsAsync(readings);

            var result = await _controller.GetSensorReadings();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetSensorReadings_ReturnsNotFound_WhenNoReadingsExist()
        {
            _mockSensorReading
                .Setup(x => x.GetSensorReadingsAsync())
                .ReturnsAsync(new List<SensorReading>());

            var result = await _controller.GetSensorReadings();

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetSensorReadingById_ReturnsOk_WhenReadingExists()
        {
            _mockSensorReading
                .Setup(x => x.GetSensorReadingByIdAsync(1))
                .ReturnsAsync(new SensorReading { Id = 1 });
            var result = await _controller.GetSensorReadingById(1);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetSensorReadingById_ReturnsNotFound_WhenReadingDoesNotExist()
        {
            _mockSensorReading
                .Setup(x => x.GetSensorReadingByIdAsync(1))
                .ReturnsAsync((SensorReading)null!);
            var result = await _controller.GetSensorReadingById(1);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetSensorReadingsBySensorId_ReturnsOk_WhenReadingsExist()
        {
            var readings = new List<SensorReading> { new() { Id = 1 } };
            _mockSensorReading
                .Setup(x => x.GetSensorReadingsBySensorIdAsync(1))
                .ReturnsAsync(readings);

            var result = await _controller.GetSensorReadingsBySensorId(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetSensorReadingsBySensorId_ReturnsNotFound_WhenNoReadingsExist()
        {
            _mockSensorReading
                .Setup(x => x.GetSensorReadingsBySensorIdAsync(1))
                .ReturnsAsync(new List<SensorReading>());

            var result = await _controller.GetSensorReadingsBySensorId(1);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteSensorReading_ReturnsOk_WhenExists()
        {
            _mockSensorReading
                .Setup(x => x.GetSensorReadingByIdAsync(1))
                .ReturnsAsync(new SensorReading { Id = 1 });
            _mockSensorReading
                .Setup(x => x.DeleteSensorReadingAsync(1))
                .Returns(Task.CompletedTask);
            var result = await _controller.DeleteSensorReading(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteSensorReading_ReturnsNotFound_WhenNotExists()
        {
            _mockSensorReading
                .Setup(x => x.GetSensorReadingByIdAsync(1))
                .ReturnsAsync((SensorReading)null!);
            var result = await _controller.DeleteSensorReading(1);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAverageSensorReadings_ReturnsOk()
        {
            var avgList = new List<AverageSensorReadingDataDTO> { new() };
            _mockSensorReading
                .Setup(x => x.GetAverageSensorReadingsFromLast24Hours(1))
                .ReturnsAsync(avgList);

            var result = await _controller.GetAverageSensorReadings(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAverageReadingFromLast7Days_ReturnsOk()
        {
            var avgList = new List<AverageSensorReadingDataDTO> { new() };
            _mockSensorReading
                .Setup(x => x.GetAverageReadingFromLast7Days(1))
                .ReturnsAsync(avgList);

            var result = await _controller.GetAverageReadingFromLast7Days(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAverageReadingFromLast30Days_ReturnsOk()
        {
            var avgList = new List<AverageSensorReadingDataDTO> { new() };
            _mockSensorReading
                .Setup(x => x.GetAverageReadingFromLast30Days(1))
                .ReturnsAsync(avgList);

            var result = await _controller.GetAverageReadingFromLast30Days(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetSensorReadingsByDate_ReturnsOk_WhenReadingsExist()
        {
            var date = new DateTime(2023, 10, 1);
            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);

            var readings = new List<SensorReading>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            };

            _mockSensorReading
                .Setup(x => x.GetSensorReadingsByDateAsync(startOfDay, endOfDay))
                .ReturnsAsync(readings);

            var result = await _controller.GetSensorReadingsByDate(date);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(readings));
        }

        [Test]
        public async Task GetSensorReadingsByDate_ReturnsNotFound_WhenNoReadingsExist()
        {
            var date = new DateTime(2023, 10, 1);
            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);

            _mockSensorReading
                .Setup(x => x.GetSensorReadingsByDateAsync(startOfDay, endOfDay))
                .ReturnsAsync(new List<SensorReading>());

            var result = await _controller.GetSensorReadingsByDate(date);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}
