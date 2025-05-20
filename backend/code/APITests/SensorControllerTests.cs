using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class SensorControllerTests
    {
        private Mock<ISensorInterface> _mockSensorLogic;
        private SensorController _controller;
        private readonly Sensor _testSensor = new() { Id = 1, Type = "Temperature" };
        private readonly AddSensorDTO _validAddDto = new() { Type = "Temperature" };
        private readonly UpdateSensorDTO _validUpdateDto = new() { Type = "Humidity" };

        [SetUp]
        public void Setup()
        {
            _mockSensorLogic = new Mock<ISensorInterface>();
            _controller = new SensorController(_mockSensorLogic.Object);
        }

        [Test]
        public async Task GetSensors_ReturnsOkWithSensors_WhenSensorsExist()
        {
            var sensors = new List<Sensor>
            {
                new() { Id = 1, Type = "Temperature" },
                new() { Id = 2, Type = "Humidity" }
            };
            _mockSensorLogic.Setup(x => x.GetAllSensorsAsync()).ReturnsAsync(sensors);
            var result = await _controller.GetSensors();
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.EqualTo(sensors));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task GetSensors_ReturnsNotFound_WhenNoSensorsExist()
        {
            _mockSensorLogic.Setup(x => x.GetAllSensorsAsync()).ReturnsAsync(new List<Sensor>());
            var result = await _controller.GetSensors();
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            if (result.Result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
                Assert.That(notFoundResult.Value, Is.EqualTo("No sensors found."));
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }

        [Test]
        public async Task GetSensors_ReturnsNotFound_WhenSensorsIsNull()
        {
            _mockSensorLogic.Setup(x => x.GetAllSensorsAsync()).ReturnsAsync((List<Sensor>)null!);
            var result = await _controller.GetSensors();
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            if (result.Result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
                Assert.That(notFoundResult.Value, Is.EqualTo("No sensors found."));
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }

        [Test]
        public async Task GetSensorById_ReturnsOkWithSensor_WhenSensorExists()
        {
            _mockSensorLogic.Setup(x => x.GetSensorByIdAsync(1)).ReturnsAsync(_testSensor);
            var result = await _controller.GetSensorById(1);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.EqualTo(_testSensor));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task GetSensorById_ReturnsNotFound_WhenSensorDoesNotExist()
        {
            _mockSensorLogic.Setup(x => x.GetSensorByIdAsync(99)).ReturnsAsync((Sensor)null!);
            var result = await _controller.GetSensorById(99);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            if (result.Result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
                Assert.That(notFoundResult.Value, Is.EqualTo("Sensor with ID 99 not found."));
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }

        [Test]
        public async Task AddSensor_ReturnsBadRequest_WhenDtoIsEmpty()
        {
            var emptyDtoMock = new EmptyAddSensorDTO();
            var result = await _controller.AddSensor(emptyDtoMock);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            if (result.Result is BadRequestObjectResult badRequest)
            {
                Assert.That(badRequest.StatusCode, Is.EqualTo(400));
                Assert.That(badRequest.Value, Is.EqualTo("Sensor data is required."));
            }
            else
            {
                Assert.Fail("Result is not BadRequestObjectResult");
            }
        }

        [Test]
        public async Task UpdateSensor_ReturnsOk_WhenSensorIsUpdated()
        {
            var updatedSensor = new Sensor { Id = 1, Type = "Humidity" };
            _mockSensorLogic
                .Setup(x => x.UpdateSensorAsync(1, _validUpdateDto))
                .ReturnsAsync(updatedSensor);
            var result = await _controller.UpdateSensor(1, _validUpdateDto);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.EqualTo(updatedSensor));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task UpdateSensor_ReturnsNotFound_WhenSensorDoesNotExist()
        {
            _mockSensorLogic
                .Setup(x => x.UpdateSensorAsync(99, _validUpdateDto))
                .ReturnsAsync((Sensor)null!);
            var result = await _controller.UpdateSensor(99, _validUpdateDto);
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            if (result.Result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
                Assert.That(notFoundResult.Value, Is.EqualTo("Sensor with ID 99 not found."));
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }

        [Test]
        public async Task UpdateSensor_ReturnsBadRequest_WhenDtoIsEmpty()
        {
            var emptyDtoMock = new EmptyUpdateSensorDTO();
            _mockSensorLogic
                .Setup(x => x.UpdateSensorAsync(1, emptyDtoMock))
                .ReturnsAsync(new Sensor { Id = 1, Type = "Updated" });
            var result = await _controller.UpdateSensor(1, emptyDtoMock);
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            if (result.Result is BadRequestObjectResult badRequest)
            {
                Assert.That(badRequest.StatusCode, Is.EqualTo(400));
                Assert.That(badRequest.Value, Is.EqualTo("Sensor data is required."));
            }
            else
            {
                Assert.Fail("Result is not BadRequestObjectResult");
            }
        }

        [Test]
        public async Task DeleteSensor_ReturnsOk_WhenSensorIsDeleted()
        {
            _mockSensorLogic.Setup(x => x.GetSensorByIdAsync(1)).ReturnsAsync(_testSensor);
            _mockSensorLogic.Setup(x => x.DeleteSensorAsync(1)).Returns(Task.CompletedTask);
            var result = await _controller.DeleteSensor(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            if (result is OkObjectResult okResult)
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.EqualTo("Sensor deleted successfully."));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
            _mockSensorLogic.Verify(x => x.DeleteSensorAsync(1), Times.Once);
        }

        [Test]
        public async Task DeleteSensor_ReturnsNotFound_WhenSensorDoesNotExist()
        {
            _mockSensorLogic.Setup(x => x.GetSensorByIdAsync(99)).ReturnsAsync((Sensor)null!);
            var result = await _controller.DeleteSensor(99);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            if (result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
                Assert.That(notFoundResult.Value, Is.EqualTo("Sensor with ID 99 not found."));
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
            _mockSensorLogic.Verify(x => x.DeleteSensorAsync(It.IsAny<int>()), Times.Never);
        }

        // Helper test classes to avoid Moq limitations with non-virtual methods
        private class EmptyAddSensorDTO : AddSensorDTO
        {
            public new bool IsEmpty() => true;
        }

        private class EmptyUpdateSensorDTO : UpdateSensorDTO
        {
            public new bool IsEmpty() => true;
        }
    }
}
