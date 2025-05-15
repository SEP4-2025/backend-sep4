using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class PictureControllerTests
    {
        private Mock<IPictureInterface> _mockPictureLogic;
        private PictureController _controller;
        private PictureDTO _testPictureDto;
        private Picture _testPicture;

        [SetUp]
        public void Setup()
        {
            _mockPictureLogic = new Mock<IPictureInterface>();
            _controller = new PictureController(_mockPictureLogic.Object);

            _testPictureDto = new PictureDTO { PlantId = 1, Note = "Test Note" };

            _testPicture = new Picture
            {
                Id = 1,
                PlantId = 1,
                Url = "http://example.com/pic.jpg",
                Note = "Test Note"
            };
        }

        // [Test]
        // public async Task AddPicture_ReturnsCreated_WhenValid()
        // {
        //     _mockPictureLogic
        //         .Setup(x => x.AddPictureAsync(_testPictureDto))
        //         .ReturnsAsync(_testPicture);

        //     var result = await _controller.AddPicture(_testPictureDto);

        //     Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        //     var created = result.Result as CreatedAtActionResult;
        //     Assert.That(created.Value, Is.EqualTo(_testPicture));
        // }

        // [Test]
        // public async Task AddPicture_ReturnsBadRequest_WhenPictureIsEmpty()
        // {
        //     var emptyDto = new PictureDTO(); // Assume IsEmpty returns true for this

        //     var result = await _controller.AddPicture(emptyDto);

        //     Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        //     var badRequest = result.Result as BadRequestObjectResult;
        //     Assert.That(badRequest.Value, Is.EqualTo("Picture is null"));
        // }

        [Test]
        public async Task GetPicturesByPlantId_ReturnsPictures()
        {
            var pictures = new List<Picture> { _testPicture };
            _mockPictureLogic.Setup(x => x.GetPictureByPlantIdAsync(1)).ReturnsAsync(pictures);

            var result = await _controller.GetPicturesByPlantId(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(pictures));
        }

        [Test]
        public async Task UpdatePictureNote_ReturnsOk_WhenPictureExists()
        {
            _mockPictureLogic
                .Setup(x => x.GetPictureByPlantIdAsync(1))
                .ReturnsAsync(new List<Picture> { _testPicture });

            var result = await _controller.UpdatePictureNote(1, "Updated Note");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeletePicture_ReturnsNoContent_WhenPictureExists()
        {
            _mockPictureLogic
                .Setup(x => x.GetPictureByPlantIdAsync(1))
                .ReturnsAsync(new List<Picture> { _testPicture });
            _mockPictureLogic.Setup(x => x.DeletePictureAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeletePicture(1);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
