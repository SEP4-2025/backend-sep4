using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class GardenerControllerTests
    {
        private Mock<IGardenerInterface> _mockGardenerLogic;
        private GardenerController _controller;
        private readonly Gardener _testGardener = new() { Id = 1, Username = "testUser" };
        private readonly GardenerDTO _validDto = new() { Username = "testUser", Password = "password" };

        [SetUp]
        public void Setup()
        {
            _mockGardenerLogic = new Mock<IGardenerInterface>();
            _controller = new GardenerController(_mockGardenerLogic.Object);
        }

        [Test]
        public async Task GetGardeners_ReturnsOkWithGardeners_WhenGardenersExist()
        {
            var gardeners = new List<Gardener>
            {
                new() { Id = 1, Username = "user1" },
                new() { Id = 2, Username = "user2" }
            };

            _mockGardenerLogic.Setup(x => x.GetGardeners()).ReturnsAsync(gardeners);

            var result = await _controller.GetGardeners();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var returnedGardeners = okResult.Value as List<Gardener>;
            Assert.That(returnedGardeners, Is.Not.Null);
            Assert.That(returnedGardeners.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetGardeners_ReturnsNotFound_WhenNoGardenersExist()
        {
            _mockGardenerLogic.Setup(x => x.GetGardeners()).ReturnsAsync(new List<Gardener>());

            var result = await _controller.GetGardeners();

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No gardeners found."));
        }

        [Test]
        public async Task GetGardenerById_ReturnsOkWithGardener_WhenGardenerExists()
        {
            _mockGardenerLogic.Setup(x => x.GetGardenerByIdAsync(1)).ReturnsAsync(_testGardener);

            var result = await _controller.GetGardenerById(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var returnedGardener = okResult.Value as Gardener;
            Assert.That(returnedGardener, Is.Not.Null);
            Assert.That(returnedGardener.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetGardenerById_ReturnsNotFound_WhenGardenerDoesNotExist()
        {
            _mockGardenerLogic.Setup(x => x.GetGardenerByIdAsync(99)).ReturnsAsync((Gardener)null);

            var result = await _controller.GetGardenerById(99);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task AddGardenerAsync_ReturnsCreatedAtAction_WhenGardenerIsAdded()
        {
            _mockGardenerLogic.Setup(x => x.GetGardeners()).ReturnsAsync(new List<Gardener>());
            _mockGardenerLogic.Setup(x => x.AddGardenerAsync(_validDto)).ReturnsAsync(_testGardener);

            var result = await _controller.AddGardenerAsync(_validDto);

            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.ActionName, Is.EqualTo("GetGardenerById"));

            var returnedGardener = createdResult.Value as Gardener;
            Assert.That(returnedGardener, Is.Not.Null);
            Assert.That(returnedGardener.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task AddGardenerAsync_ReturnsBadRequest_WhenGardenerAlreadyExists()
        {
            var existingGardeners = new List<Gardener> { new() { Id = 1, Username = "existing" } };
            _mockGardenerLogic.Setup(x => x.GetGardeners()).ReturnsAsync(existingGardeners);

            var result = await _controller.AddGardenerAsync(_validDto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("You can only create one gardener."));
        }

        [Test]
        public async Task AddGardenerAsync_ReturnsBadRequest_WhenDtoIsEmpty()
        {
            var result = await _controller.AddGardenerAsync(new GardenerDTO());

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest.StatusCode, Is.EqualTo(400));
            Assert.That(badRequest.Value, Is.EqualTo("Gardener data is required."));
        }

        [Test]
        public async Task UpdateGardener_ReturnsBadRequest_WhenDtoHasNoValues()
        {
            var result = await _controller.UpdateGardener(1, new GardenerDTO());

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest.StatusCode, Is.EqualTo(400));
            Assert.That(badRequest.Value, Is.EqualTo("At least one field must be provided to update."));
        }

        [Test]
        public async Task UpdateGardener_ReturnsOk_WhenGardenerIsUpdated()
        {
            var updateDto = new GardenerDTO { Username = "updatedUser" };
            _mockGardenerLogic.Setup(x => x.GetGardenerByIdAsync(1)).ReturnsAsync(_testGardener);

            var result = await _controller.UpdateGardener(1, updateDto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateGardener_ReturnsNotFound_WhenGardenerDoesNotExist()
        {
            _mockGardenerLogic.Setup(x => x.GetGardenerByIdAsync(42)).ReturnsAsync((Gardener)null);

            var result = await _controller.UpdateGardener(42, new GardenerDTO { Username = "irrelevant" });

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteGardener_ReturnsOk_WhenGardenerIsDeleted()
        {
            _mockGardenerLogic.Setup(x => x.GetGardenerByIdAsync(1)).ReturnsAsync(_testGardener);
            _mockGardenerLogic.Setup(x => x.DeleteGardenerAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteGardener(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("Gardener deleted successfully."));
        }

        [Test]
        public async Task DeleteGardener_ReturnsNotFound_WhenGardenerDoesNotExist()
        {
            _mockGardenerLogic.Setup(x => x.GetGardenerByIdAsync(42)).ReturnsAsync((Gardener)null);

            var result = await _controller.DeleteGardener(42);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Gardener with ID 42 not found."));
        }

        [Test]
        public async Task DeleteGardener_ReturnsBadRequest_WhenGardenerIsLinkedToGreenhouse()
        {
            var innerException = new Exception("23503: insert or update on table \"Greenhouse\" violates foreign key constraint \"Greenhouse_gardenerid_fkey\"");
            var dbUpdateException = new DbUpdateException("Foreign key constraint", innerException);

            _mockGardenerLogic.Setup(x => x.GetGardenerByIdAsync(1)).ReturnsAsync(_testGardener);
            _mockGardenerLogic.Setup(x => x.DeleteGardenerAsync(1)).ThrowsAsync(dbUpdateException);

            var result = await _controller.DeleteGardener(1);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest.StatusCode, Is.EqualTo(400));
            Assert.That(badRequest.Value, Is.EqualTo("Cannot delete Gardener because it is associated with a Greenhouse."));
        }

        [Test]
        public void DeleteGardener_ThrowsException_WhenDbUpdateExceptionIsUnrelated()
        {
            var unrelatedInnerException = new Exception("Some other DB error");
            var unrelatedDbUpdateException = new DbUpdateException("Unrelated DB error", unrelatedInnerException);

            _mockGardenerLogic.Setup(x => x.GetGardenerByIdAsync(1)).ReturnsAsync(_testGardener);
            _mockGardenerLogic.Setup(x => x.DeleteGardenerAsync(1)).ThrowsAsync(unrelatedDbUpdateException);

            Assert.ThrowsAsync<DbUpdateException>(async () => await _controller.DeleteGardener(1));
        }
    }
}