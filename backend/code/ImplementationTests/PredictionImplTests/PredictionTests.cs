using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Entities;
using LogicImplements;
using LogicInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using DTOs;

namespace ImplementationTests.PredictionImplTests;

public class PredictionTests
{
    
    private AppDbContext _context;
    private Mock<IHttpClientFactory> _httpFactoryMock;
    private IPredictionInterface _predictionLogic;

        [SetUp]
        public void Setup()
        {
            // 1) Database in-memory
            _context = TestSetup.Context;

            // 2) Mock IHttpClientFactory + handler
            _httpFactoryMock = new Mock<IHttpClientFactory>();

            // 3) Configuration stub
            var inMemorySettings = new Dictionary<string, string> {
                {"PythonApi:BaseUrl", "http://fake-api.local" }
            };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // 4) Logger stub
            var logger = Mock.Of<ILogger<PredictionLogic>>();

            // 5) Instantiate logic under test
            _predictionLogic = new PredictionLogic(
                _context,
                _httpFactoryMock.Object,
                config,
                logger
            );
        }

        [Test]
        public async Task RepredictLatestAsync_SendsLatestToApiAndReturnsResponse()
        {
            // --- Arrange ---
            // Seed two predictions with distinct dates
            var older = await PredictionSeeder.SeedPredictionAsync(date: DateTime.UtcNow.AddMinutes(-10));
            var newer = await PredictionSeeder.SeedPredictionAsync(date: DateTime.UtcNow);

            // Prepare fake response from Python API
            var fakeResponseDto = new PredictionResponseDTO
            {
                Prediction     = 123.45,
                PredictionProba= new List<double>{0.9, 0.1},
                InputReceived  = new PredictionRequestDTO {
                    Temperature     = newer.Temperature,
                    Light           = newer.Light,
                    AirHumidity     = newer.AirHumidity,
                    SoilHumidity    = newer.SoilHumidity,
                    Date            = newer.Date,
                    GreenhouseId    = newer.GreenhouseId,
                    SensorReadingId = newer.SensorReadingId
                }
            };
            var fakeJson = JsonContent.Create(fakeResponseDto);

            // Mock a handler that intercepts POST and returns our fake JSON
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.Is<HttpRequestMessage>(req =>
                       req.Method == HttpMethod.Post &&
                       req.RequestUri == new Uri("http://fake-api.local/predict")),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage {
                   StatusCode = HttpStatusCode.OK,
                   Content    = fakeJson
               })
               .Verifiable();

            var client = new HttpClient(handlerMock.Object);
            _httpFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(client);

            // --- Act ---
            var result = await _predictionLogic.RepredictLatestAsync();

            // --- Assert ---
            Assert.IsNotNull(result, "Result should not be null");
            Assert.That(result.Prediction, Is.EqualTo(fakeResponseDto.Prediction));
            Assert.That(result.PredictionProba, Is.EqualTo(fakeResponseDto.PredictionProba));
            // Ensure that the InputReceived matches our newest seed
            Assert.That(result.InputReceived.SensorReadingId, Is.EqualTo(newer.SensorReadingId));

            // And that we indeed hit our fake handler
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("http://fake-api.local/predict")
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }


    [TearDown]
    public async Task TearDown()
    {
        _context.Predictions.RemoveRange(_context.Predictions);
        await _context.SaveChangesAsync();
    }
}
