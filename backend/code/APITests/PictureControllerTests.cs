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

            var mockFile = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1);
            _testPictureDto = new PictureDTO
            {
                PlantId = 1,
                File = mockFile.Object,
                Note = "Test Note"
            };

            _testPicture = new Picture
            {
                Id = 1,
                PlantId = 1,
                Url = "http://example.com/pic.jpg",
                Note = "Test Note"
            };
        }

        [Test]
        public async Task AddPicture_ReturnsCreated_WhenValid()
        {
            _mockPictureLogic
                .Setup(x => x.AddPictureAsync(_testPictureDto))
                .ReturnsAsync(_testPicture);

            var result = await _controller.AddPicture(_testPictureDto);

            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            if (result.Result is CreatedAtActionResult created)
            {
                Assert.That(created.Value, Is.EqualTo(_testPicture));
            }
            else
            {
                Assert.Fail("Result is not CreatedAtActionResult");
            }
        }

        [Test]
        public async Task AddPicture_ReturnsBadRequest_WhenPictureIsEmpty()
        {
            var emptyDto = new PictureDTO();

            var result = await _controller.AddPicture(emptyDto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            if (result.Result is BadRequestObjectResult badRequest)
            {
                Assert.That(badRequest.Value, Is.EqualTo("Picture data is invalid"));
            }
            else
            {
                Assert.Fail("Result is not BadRequestObjectResult");
            }
        }

        [Test]
        public async Task GetPicturesByPlantId_ReturnsPictures()
        {
            var pictures = new List<Picture> { _testPicture };
            _mockPictureLogic.Setup(x => x.GetPictureByPlantIdAsync(1)).ReturnsAsync(pictures);

            var result = await _controller.GetPicturesByPlantId(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(pictures));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task GetPicturesByPlantId_ReturnsEmptyList_WhenNoPictures()
        {
            _mockPictureLogic
                .Setup(x => x.GetPictureByPlantIdAsync(2))
                .ReturnsAsync(new List<Picture>());

            var result = await _controller.GetPicturesByPlantId(2);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                var value = okResult.Value as List<Picture>;
                Assert.That(value, Is.Not.Null);
                Assert.That(value.Count, Is.EqualTo(0));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task UpdatePictureNote_ReturnsOk_WhenPictureExists()
        {
            _mockPictureLogic
                .Setup(x => x.GetPictureByPlantIdAsync(1))
                .ReturnsAsync(new List<Picture> { _testPicture });
            _mockPictureLogic
                .Setup(x => x.UpdateNote(1, "Updated Note"))
                .ReturnsAsync(_testPicture);

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

        [Test]
        public async Task DeletePicture_ReturnsNotFound_WhenPictureDoesNotExist()
        {
            _mockPictureLogic
                .Setup(x => x.GetPictureByPlantIdAsync(99))
                .ReturnsAsync((List<Picture>)null!);

            var result = await _controller.DeletePicture(99);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            if (result is NotFoundObjectResult notFound)
            {
                Assert.That(notFound.Value, Is.EqualTo("Picture with id 99 not found"));
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }
    }
}
