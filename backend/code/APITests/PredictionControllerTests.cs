using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class PredictionControllerTests
    {
        private Mock<IPredictionInterface> _mockPredictionLogic;
        private PredictionController _controller;
        private readonly Prediction _testPrediction =
            new()
            {
                Id = 1,
                Date = DateTime.Today,
                SensorReadingId = 1
            };
        private readonly PredictionDTO _testPredictionDto = new() { SensorReadingId = 1 };

        [SetUp]
        public void Setup()
        {
            _mockPredictionLogic = new Mock<IPredictionInterface>();
            _controller = new PredictionController(_mockPredictionLogic.Object);
        }

        [Test]
        public async Task GetPredictionById_ReturnsOk_WhenPredictionExists()
        {
            _mockPredictionLogic
                .Setup(x => x.GetPredictionByIdAsync(1))
                .ReturnsAsync(_testPrediction);

            var result = await _controller.GetPredictionById(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(_testPrediction));
        }

        [Test]
        public async Task GetPredictionById_ReturnsNotFound_WhenPredictionDoesNotExist()
        {
            _mockPredictionLogic
                .Setup(x => x.GetPredictionByIdAsync(99))
                .ReturnsAsync((Prediction)null);

            var result = await _controller.GetPredictionById(99);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No prediction found with id 99"));
        }

        [Test]
        public async Task GetPredictionsByDate_ReturnsOkWithPredictions_WhenPredictionsExist()
        {
            var date = DateTime.Today;
            var predictions = new List<Prediction> { _testPrediction };
            _mockPredictionLogic
                .Setup(x => x.GetPredictionsByDateAsync(date))
                .ReturnsAsync(predictions);

            var result = await _controller.GetPredictionsByDate(date);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(predictions));
        }

        [Test]
        public async Task GetAllPredictions_ReturnsOkWithPredictions()
        {
            var predictions = new List<Prediction> { _testPrediction };
            _mockPredictionLogic.Setup(x => x.GetAllPredictions()).ReturnsAsync(predictions);

            var result = await _controller.GetAllPredictions();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(predictions));
        }

        [Test]
        public async Task AddPrediction_ReturnsOk_WhenPredictionIsValid()
        {
            _mockPredictionLogic
                .Setup(x => x.AddPredictionAsync(_testPredictionDto))
                .Returns(Task.CompletedTask);

            var result = await _controller.AddPrediction(_testPredictionDto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(_testPredictionDto));
        }

        [Test]
        public async Task AddPrediction_ReturnsBadRequest_WhenPredictionIsNull()
        {
            var result = await _controller.AddPrediction(null);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Prediction cannot be null"));
        }

        [Test]
        public async Task DeletePrediction_ReturnsOk_WhenPredictionExists()
        {
            _mockPredictionLogic
                .Setup(x => x.GetPredictionByIdAsync(1))
                .ReturnsAsync(_testPrediction);
            _mockPredictionLogic.Setup(x => x.DeletePredictionAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeletePrediction(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("Prediction with id 1 deleted"));
        }

        [Test]
        public async Task DeletePrediction_ReturnsNotFound_WhenPredictionDoesNotExist()
        {
            _mockPredictionLogic
                .Setup(x => x.GetPredictionByIdAsync(99))
                .ReturnsAsync((Prediction)null);

            var result = await _controller.DeletePrediction(99);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No prediction found with id 99"));
        }
    }
}
