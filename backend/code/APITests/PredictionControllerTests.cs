using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DTOs;               // PredictionResponseDTO, PredictionRequestDTO
using Entities;           // Prediction
using LogicInterfaces;    // IPredictionInterface
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class PredictionControllerRepredictTests
    {
        private Mock<IPredictionInterface> _logicMock;
        private Mock<IHttpClientFactory> _httpFactoryMock;
        private PredictionController _controller;
        private PredictionResponseDTO _fakeResponseDto;

        [SetUp]
        public void SetUp()
        {
            // Arrange raw prediction returned by logic
            var rawPrediction = new Prediction
            {
                Temperature = 10,
                Light = 20,
                AirHumidity = 30,
                SoilHumidity = 40,
                Date = DateTime.UtcNow,
                GreenhouseId = 1,
                SensorReadingId = 99
            };

            // Arrange fake ML service response
            _fakeResponseDto = new PredictionResponseDTO
            {
                Prediction = 42.0,
                PredictionProba = null,
                InputReceived = new PredictionRequestDTO
                {
                    Temperature = rawPrediction.Temperature,
                    Light = rawPrediction.Light,
                    AirHumidity = rawPrediction.AirHumidity,
                    SoilHumidity = rawPrediction.SoilHumidity,
                    Date = rawPrediction.Date,
                    GreenhouseId = rawPrediction.GreenhouseId,
                    SensorReadingId = rawPrediction.SensorReadingId
                }
            };

            // Mock business logic
            _logicMock = new Mock<IPredictionInterface>();
            _logicMock
                .Setup(x => x.GetLastPredictionAsync())
                .ReturnsAsync(rawPrediction);

            // Mock HttpClient to return fake response
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.PathAndQuery == "/predict"),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(
                        JsonSerializer.Serialize(_fakeResponseDto),
                        Encoding.UTF8,
                        "application/json"
                    )
                });

            var client = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _httpFactoryMock = new Mock<IHttpClientFactory>();
            _httpFactoryMock
                .Setup(f => f.CreateClient("ml-api"))
                .Returns(client);

            // Instantiate controller
            var logger = Mock.Of<ILogger<PredictionController>>();
            _controller = new PredictionController(
                _logicMock.Object,
                logger,
                _httpFactoryMock.Object
            );
        }

        [Test]
        public async Task RepredictLatest_ReturnsDto_WhenLogicSucceeds()
        {
            var result = await _controller.RepredictLatest();

            // The controller returns the DTO directly, wrapped by MVC as Value
            Assert.That(result.Value, Is.EqualTo(_fakeResponseDto).Using<PredictionResponseDTO>(
                (a, b) => a.Prediction == b.Prediction
                          && a.PredictionProba == b.PredictionProba
                          && a.InputReceived.Temperature == b.InputReceived.Temperature
                          && a.InputReceived.Light == b.InputReceived.Light
                          && a.InputReceived.AirHumidity == b.InputReceived.AirHumidity
                          && a.InputReceived.SoilHumidity == b.InputReceived.SoilHumidity
                          && a.InputReceived.Date.ToUniversalTime() == b.InputReceived.Date.ToUniversalTime()
                          && a.InputReceived.GreenhouseId == b.InputReceived.GreenhouseId
                          && a.InputReceived.SensorReadingId == b.InputReceived.SensorReadingId
            ));

            Assert.That(result.Result, Is.Null);
        }

        [Test]
        public void RepredictLatest_ThrowsInvalidOperation_WhenLogicThrows()
        {
            _logicMock
                .Setup(x => x.GetLastPredictionAsync())
                .ThrowsAsync(new InvalidOperationException("no data"));

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _controller.RepredictLatest()
            );
        }

        [Test]
        public async Task RepredictLatest_ThrowsHttpRequestException_WhenHttpFails()
        {
            // 1) arrange: stub the factory to give a client whose handler throws
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("connect fail"));

            var failingClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            _httpFactoryMock
                .Setup(f => f.CreateClient("ml-api"))
                .Returns(failingClient);

            // 2) re‐new the controller so it picks up the new client
            var logger = Mock.Of<ILogger<PredictionController>>();
            var controller = new PredictionController(
                _logicMock.Object,
                logger,
                _httpFactoryMock.Object
            );

            // 3) assert
            Assert.ThrowsAsync<HttpRequestException>(() =>
               controller.RepredictLatest()
           );
        }

        [Test]
        public void RepredictLatest_ThrowsException_WhenUnexpectedError()
        {
            _logicMock
                .Setup(x => x.GetLastPredictionAsync())
                .ThrowsAsync(new Exception("boom"));

            Assert.ThrowsAsync<Exception>(
                async () => await _controller.RepredictLatest()
            );
        }
    }
}
