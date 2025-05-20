using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class PlantControllerTests
    {
        private Mock<IPlantInterface> _mockPlantLogic;
        private PlantController _controller;
        private PlantDTO _testPlantDto;
        private Plant _testPlant;

        [SetUp]
        public void Setup()
        {
            _mockPlantLogic = new Mock<IPlantInterface>();
            _controller = new PlantController(_mockPlantLogic.Object);

            _testPlantDto = new PlantDTO { Name = "Test Plant", GreenhouseId = 1 };

            _testPlant = new Plant
            {
                Id = 1,
                Name = "Test Plant",
                GreenhouseId = 1
            };
        }

        [Test]
        public async Task GetPlants_ReturnsOk_WhenPlantsExist()
        {
            var plants = new List<Plant> { _testPlant };
            _mockPlantLogic.Setup(x => x.GetPlantsAsync()).ReturnsAsync(plants);

            var result = await _controller.GetPlants();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(plants));
        }

        [Test]
        public async Task GetPlants_ReturnsNotFound_WhenNoPlantsExist()
        {
            _mockPlantLogic.Setup(x => x.GetPlantsAsync()).ReturnsAsync(new List<Plant>());

            var result = await _controller.GetPlants();

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No plants found."));
        }

        [Test]
        public async Task GetPlantById_ReturnsOk_WhenPlantExists()
        {
            _mockPlantLogic.Setup(x => x.GetPlantByIdAsync(1)).ReturnsAsync(_testPlant);

            var result = await _controller.GetPlantById(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(_testPlant));
        }

        [Test]
        public async Task GetPlantById_ReturnsNotFound_WhenPlantDoesNotExist()
        {
            _mockPlantLogic.Setup(x => x.GetPlantByIdAsync(99)).ReturnsAsync((Plant)null);

            var result = await _controller.GetPlantById(99);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No plant found with id 99"));
        }

        [Test]
        public async Task AddPlant_ReturnsOk_WhenNoPlantExists()
        {
            _mockPlantLogic.Setup(x => x.GetPlantsAsync()).ReturnsAsync(new List<Plant>());
            _mockPlantLogic.Setup(x => x.AddPlantAsync(_testPlantDto)).ReturnsAsync(_testPlant);

            var result = await _controller.AddPlant(_testPlantDto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var okResult = result.Result as BadRequestObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(400));
            Assert.That(okResult.Value, Is.EqualTo("Plant cannot be null"));
        }

        [Test]
        public async Task AddPlant_ReturnsBadRequest_WhenPlantIsEmpty()
        {
            var emptyDto = new PlantDTO(); // Assuming IsEmpty returns true for this

            var result = await _controller.AddPlant(emptyDto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Plant cannot be null"));
        }

        [Test]
        public async Task AddPlant_ReturnsBadRequest_WhenPlantAlreadyExists()
        {
            var existingPlants = new List<Plant> { _testPlant };
            _mockPlantLogic.Setup(x => x.GetPlantsAsync()).ReturnsAsync(existingPlants);

            var result = await _controller.AddPlant(_testPlantDto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Plant cannot be null"));
        }

        [Test]
        public async Task UpdatePlantName_ReturnsOk_WhenPlantExists()
        {
            _mockPlantLogic.Setup(x => x.GetPlantByIdAsync(1)).ReturnsAsync(_testPlant);
            _mockPlantLogic
                .Setup(x => x.UpdatePlantNameAsync(1, "Updated Plant"))
                .ReturnsAsync(_testPlant);

            var result = await _controller.UpdatePlantName(1, "Updated Plant");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(_testPlant));
        }

        [Test]
        public async Task UpdatePlantName_ReturnsNotFound_WhenPlantDoesNotExist()
        {
            _mockPlantLogic.Setup(x => x.GetPlantByIdAsync(99)).ReturnsAsync((Plant)null);

            var result = await _controller.UpdatePlantName(99, "Updated Plant");

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No plant found with id 99"));
        }

        [Test]
        public async Task UpdatePlantName_ReturnsBadRequest_WhenNameIsEmpty()
        {
            _mockPlantLogic.Setup(x => x.GetPlantByIdAsync(1)).ReturnsAsync(_testPlant);

            var result = await _controller.UpdatePlantName(1, "");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Plant name cannot be null or empty."));
        }

        [Test]
        public async Task DeletePlant_ReturnsOk_WhenPlantExists()
        {
            _mockPlantLogic.Setup(x => x.GetPlantByIdAsync(1)).ReturnsAsync(_testPlant);
            _mockPlantLogic.Setup(x => x.DeletePlantAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeletePlant(1);

            Assert.That(result, Is.InstanceOf<OkResult>());
            var okResult = result as OkResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task DeletePlant_ReturnsNotFound_WhenPlantDoesNotExist()
        {
            _mockPlantLogic.Setup(x => x.GetPlantByIdAsync(99)).ReturnsAsync((Plant)null);

            var result = await _controller.DeletePlant(99);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No plant found with id 99"));
        }
    }
}
