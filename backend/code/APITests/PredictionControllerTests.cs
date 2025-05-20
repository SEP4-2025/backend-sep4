using System;
using System.Net.Http;
using System.Threading.Tasks;
using DTOs;  // PredictionResponseDTO
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class PredictionControllerRepredictTests
    {
        private Mock<IPredictionInterface> _logicMock;
        private PredictionController      _controller;

        [SetUp]
        public void SetUp()
        {
            // 1) mock the business logic
            _logicMock   = new Mock<IPredictionInterface>();
            // 2) stub a logger
            var logger = Mock.Of<ILogger<PredictionController>>();
            // 3) instantiate controller under test
            _controller = new PredictionController(_logicMock.Object, logger);
        }

        [Test]
        public async Task RepredictLatest_ReturnsOk_WhenLogicSucceeds()
        {
            // arrange
            var dto = new PredictionResponseDTO
            {
                Prediction      = 42.0,
                PredictionProba = null,
                InputReceived   = new PredictionRequestDTO
                {
                    Temperature     = 10,
                    Light           = 20,
                    AirHumidity     = 30,
                    SoilHumidity    = 40,
                    Date            = DateTime.UtcNow,
                    GreenhouseId    = 1,
                    SensorReadingId = 99
                }
            };
            _logicMock
                .Setup(x => x.RepredictLatestAsync())
                .ReturnsAsync(dto);

            // act
            var actionResult = await _controller.RepredictLatest();

            // assert
            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)actionResult.Result;
            Assert.That(ok.StatusCode, Is.EqualTo(200));
            Assert.That(ok.Value, Is.SameAs(dto));
        }

        [Test]
        public async Task RepredictLatest_ReturnsNotFound_WhenLogicThrowsInvalidOperation()
        {
            // arrange
            _logicMock
                .Setup(x => x.RepredictLatestAsync())
                .ThrowsAsync(new InvalidOperationException("no data"));

            // act
            var actionResult = await _controller.RepredictLatest();

            // assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundObjectResult>());
            var nf = (NotFoundObjectResult)actionResult.Result;
            Assert.That(nf.StatusCode, Is.EqualTo(404));
            Assert.That(nf.Value, Is.EqualTo("no data"));
        }

        [Test]
        public async Task RepredictLatest_ReturnsBadGateway_WhenLogicThrowsHttpRequest()
        {
            // arrange
            _logicMock
                .Setup(x => x.RepredictLatestAsync())
                .ThrowsAsync(new HttpRequestException("connect fail"));

            // act
            var actionResult = await _controller.RepredictLatest();

            // assert
            Assert.That(actionResult.Result, Is.InstanceOf<ObjectResult>());
            var br = (ObjectResult)actionResult.Result;
            Assert.That(br.StatusCode, Is.EqualTo(502));
            Assert.That(br.Value, Is.EqualTo("connect fail"));
        }

        [Test]
        public async Task RepredictLatest_ReturnsServerError_WhenLogicThrowsUnexpected()
        {
            // arrange
            _logicMock
                .Setup(x => x.RepredictLatestAsync())
                .ThrowsAsync(new Exception("boom"));

            // act
            var actionResult = await _controller.RepredictLatest();

            // assert
            Assert.That(actionResult.Result, Is.InstanceOf<ObjectResult>());
            var er = (ObjectResult)actionResult.Result;
            Assert.That(er.StatusCode, Is.EqualTo(500));
            // controller does ex.ToString(), so we at least check it contains the message
            Assert.That(er.Value.ToString(), Does.Contain("boom"));
        }
    }
}