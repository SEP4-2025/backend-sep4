using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class GreenhouseControllerTests
    {
        private Mock<IGreenhouseInterface> _mockGreenhouseLogic;
        private GreenhouseController _controller;
        private readonly Greenhouse _testGreenhouse = new() { Id = 1, Name = "TestGreenhouse", GardenerId = 1 };
        private readonly GreenhouseDTO _validDto = new() { Name = "TestGreenhouse", GardenerId = 1 };

        [SetUp]
        public void Setup()
        {
            _mockGreenhouseLogic = new Mock<IGreenhouseInterface>();
            _controller = new GreenhouseController(_mockGreenhouseLogic.Object);
        }

        [Test]
        public async Task GetAllGreenhousesAsync_ReturnsOkWithGreenhouses_WhenGreenhousesExist()
        {
            var greenhouses = new List<Greenhouse>
            {
                new() { Id = 1, Name = "Greenhouse1", GardenerId = 1 },
                new() { Id = 2, Name = "Greenhouse2", GardenerId = 2 }
            };

            _mockGreenhouseLogic.Setup(x => x.GetGreenhouses()).ReturnsAsync(greenhouses);

            var result = await _controller.GetAllGreenhousesAsync();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var returnedGreenhouses = okResult.Value as List<Greenhouse>;
            Assert.That(returnedGreenhouses, Is.Not.Null);
            Assert.That(returnedGreenhouses.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllGreenhousesAsync_ReturnsNotFound_WhenNoGreenhousesExist()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouses()).ReturnsAsync(new List<Greenhouse>());

            var result = await _controller.GetAllGreenhousesAsync();

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No greenhouses found."));
        }

        [Test]
        public async Task GetGreenhouseByIdAsync_ReturnsOkWithGreenhouse_WhenGreenhouseExists()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(1)).ReturnsAsync(_testGreenhouse);

            var result = await _controller.GetGreenhouseByIdAsync(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var returnedGreenhouse = okResult.Value as Greenhouse;
            Assert.That(returnedGreenhouse, Is.Not.Null);
            Assert.That(returnedGreenhouse.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetGreenhouseByIdAsync_ReturnsNotFound_WhenGreenhouseDoesNotExist()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(99)).ReturnsAsync((Greenhouse)null);

            var result = await _controller.GetGreenhouseByIdAsync(99);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Greenhouse with id 99 not found."));
        }

        [Test]
        public async Task GetGreenhouseByGardenerId_ReturnsOkWithGreenhouse_WhenGreenhouseExists()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseByGardenerId(1)).ReturnsAsync(_testGreenhouse);

            var result = await _controller.GetGreenhouseByGardenerId(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var returnedGreenhouse = okResult.Value as Greenhouse;
            Assert.That(returnedGreenhouse, Is.Not.Null);
            Assert.That(returnedGreenhouse.GardenerId, Is.EqualTo(1));
        }

        [Test]
        public async Task GetGreenhouseByGardenerId_ReturnsNotFound_WhenGreenhouseDoesNotExist()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseByGardenerId(99)).ReturnsAsync((Greenhouse)null);

            var result = await _controller.GetGreenhouseByGardenerId(99);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Greenhouse with gardenerId 99 does not exist."));
        }

        [Test]
        public async Task GetGreenhouseByNameAsync_ReturnsOkWithGreenhouse_WhenGreenhouseExists()
        {
            string greenhouseName = "TestGreenhouse";
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseByName(greenhouseName)).ReturnsAsync(_testGreenhouse);

            var result = await _controller.GetGreenhouseByNameAsync(greenhouseName);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var returnedGreenhouse = okResult.Value as Greenhouse;
            Assert.That(returnedGreenhouse, Is.Not.Null);
            Assert.That(returnedGreenhouse.Name, Is.EqualTo(greenhouseName));
        }

        [Test]
        public async Task GetGreenhouseByNameAsync_ReturnsNotFound_WhenGreenhouseDoesNotExist()
        {
            string nonExistentName = "NonExistentGreenhouse";
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseByName(nonExistentName)).ReturnsAsync((Greenhouse)null);

            var result = await _controller.GetGreenhouseByNameAsync(nonExistentName);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo($"Greenhouse with name {nonExistentName} does not exist."));
        }

        [Test]
        public async Task AddGreenhouseAsync_ReturnsOk_WhenGreenhouseIsAdded()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouses()).ReturnsAsync(new List<Greenhouse>());
            _mockGreenhouseLogic.Setup(x => x.AddGreenhouse(_validDto)).ReturnsAsync(_testGreenhouse);

            var result = await _controller.AddGreenhouseAsync(_validDto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var returnedGreenhouse = okResult.Value as Greenhouse;
            Assert.That(returnedGreenhouse, Is.Not.Null);
            Assert.That(returnedGreenhouse.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task AddGreenhouseAsync_ReturnsBadRequest_WhenDtoIsEmpty()
        {
            var emptyDto = new GreenhouseDTO();
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouses()).ReturnsAsync(new List<Greenhouse>());

            var result = await _controller.AddGreenhouseAsync(emptyDto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Gardener data is required."));
        }

        [Test]
        public async Task AddGreenhouseAsync_ReturnsBadRequest_WhenGreenhouseAlreadyExists()
        {
            var existingGreenhouses = new List<Greenhouse> { new() { Id = 1, Name = "ExistingGreenhouse", GardenerId = 1 } };
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouses()).ReturnsAsync(existingGreenhouses);

            var result = await _controller.AddGreenhouseAsync(_validDto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("You can only create one greenhouse."));
        }

        [Test]
        public async Task UpdateGreenhouseNameAsync_ReturnsOk_WhenGreenhouseIsUpdated()
        {
            string newName = "UpdatedGreenhouse";
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(1)).ReturnsAsync(_testGreenhouse);

            var result = await _controller.UpdateGreenhouseNameAsync(1, newName);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateGreenhouseNameAsync_ReturnsNotFound_WhenGreenhouseDoesNotExist()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(99)).ReturnsAsync((Greenhouse)null);

            var result = await _controller.UpdateGreenhouseNameAsync(99, "NewName");

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Greenhouse with id 99 not found."));
        }

        [Test]
        public async Task UpdateGreenhouseNameAsync_ReturnsBadRequest_WhenNameIsEmpty()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(1)).ReturnsAsync(_testGreenhouse);

            var result = await _controller.UpdateGreenhouseNameAsync(1, string.Empty);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Name cannot be empty."));
        }

        [Test]
        public async Task DeleteGreenhouseAsync_ReturnsOk_WhenGreenhouseIsDeleted()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(1)).ReturnsAsync(_testGreenhouse);
            _mockGreenhouseLogic.Setup(x => x.DeleteGreenhouse(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteGreenhouseAsync(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("Successfully deleted greenhouse."));
        }

        [Test]
        public async Task DeleteGreenhouseAsync_ReturnsNotFound_WhenGreenhouseDoesNotExist()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(99)).ReturnsAsync((Greenhouse)null);

            var result = await _controller.DeleteGreenhouseAsync(99);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Greenhouse with id 99 not found."));
        }

        [Test]
        public async Task DeleteGreenhouseAsync_ReturnsBadRequest_WhenGreenhouseHasPlants()
        {
            var exception = new Exception("Plant_greenhouseid_fkey");

            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(1)).ReturnsAsync(_testGreenhouse);
            _mockGreenhouseLogic.Setup(x => x.DeleteGreenhouse(1)).ThrowsAsync(
                new Exception("Error", new Exception("Plant_greenhouseid_fkey")));

            var result = await _controller.DeleteGreenhouseAsync(1);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Cannot delete Greenhouse because it is associated with a Plant."));
        }

        [Test]
        public async Task DeleteGreenhouseAsync_ReturnsBadRequest_WhenGreenhouseHasSensors()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(1)).ReturnsAsync(_testGreenhouse);
            _mockGreenhouseLogic.Setup(x => x.DeleteGreenhouse(1)).ThrowsAsync(
                new Exception("Error", new Exception("Sensor_greenhouseid_fkey")));

            var result = await _controller.DeleteGreenhouseAsync(1);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Cannot delete Greenhouse because it has associated Sensors."));
        }

        [Test]
        public void DeleteGreenhouseAsync_ThrowsException_WhenExceptionIsUnrelated()
        {
            _mockGreenhouseLogic.Setup(x => x.GetGreenhouseById(1)).ReturnsAsync(_testGreenhouse);
            _mockGreenhouseLogic.Setup(x => x.DeleteGreenhouse(1)).ThrowsAsync(
                new Exception("Some other error"));

            Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteGreenhouseAsync(1));
        }
    }
}