using Entities;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;

namespace APITests
{
    [TestFixture]
    public class LogControllerTests
    {
        private Mock<ILogInterface> _mockLogLogic;
        private LogController _controller;
        private readonly Log _testLog =
            new()
            {
                Id = 1,
                Timestamp = new DateTime(2025, 5, 1),
                Message = "Test log"
            };
        private readonly DateTime _testDate = new DateTime(2025, 5, 1);

        [SetUp]
        public void Setup()
        {
            _mockLogLogic = new Mock<ILogInterface>();
            _controller = new LogController(_mockLogLogic.Object);
        }

        [Test]
        public async Task GetLogs_ReturnsAllLogs()
        {
            var logs = new List<Log>
            {
                new()
                {
                    Id = 1,
                    Timestamp = new DateTime(2025, 5, 1),
                    Message = "Log 1"
                },
                new()
                {
                    Id = 2,
                    Timestamp = new DateTime(2025, 5, 2),
                    Message = "Log 2"
                }
            };

            _mockLogLogic.Setup(x => x.GetAllLogs()).ReturnsAsync(logs);

            var result = await _controller.GetLogs();

            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetLogsByDate_ReturnsLogs_WhenLogsExistForDate()
        {
            var logs = new List<Log>
            {
                new()
                {
                    Id = 1,
                    Timestamp = _testDate,
                    Message = "Log 1"
                },
                new()
                {
                    Id = 2,
                    Timestamp = _testDate,
                    Message = "Log 2"
                }
            };

            _mockLogLogic.Setup(x => x.GetLogsByDateAsync(_testDate)).ReturnsAsync(logs);

            var result = await _controller.GetLogsByDate(_testDate);

            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetLogsByDate_ReturnsNotFound_WhenNoLogsExistForDate()
        {
            DateTime dateWithNoLogs = new DateTime(2025, 6, 1);

            _mockLogLogic
                .Setup(x => x.GetLogsByDateAsync(dateWithNoLogs))
                .ReturnsAsync((List<Log>)null!);

            var result = await _controller.GetLogsByDate(dateWithNoLogs);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            if (result.Result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
                Assert.That(
                    notFoundResult.Value,
                    Is.EqualTo($"No logs found for date {dateWithNoLogs}")
                );
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }

        [Test]
        public async Task DeleteLog_ReturnsNoContent_WhenLogExists()
        {
            _mockLogLogic.Setup(x => x.GetLogByIdAsync(1)).ReturnsAsync(_testLog);
            _mockLogLogic.Setup(x => x.DeleteLogAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteLog(1);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
            if (result is NoContentResult noContentResult)
            {
                Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
            }
            else
            {
                Assert.Fail("Result is not NoContentResult");
            }
        }

        [Test]
        public async Task DeleteLog_ReturnsNotFound_WhenLogDoesNotExist()
        {
            int nonExistentId = 99;

            _mockLogLogic.Setup(x => x.GetLogByIdAsync(nonExistentId)).ReturnsAsync((Log)null!);

            var result = await _controller.DeleteLog(nonExistentId);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            if (result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
                Assert.That(
                    notFoundResult.Value,
                    Is.EqualTo($"Log with id {nonExistentId} not found")
                );
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }

        // Additional tests for 100% coverage
        [Test]
        public async Task GetWaterUsageForLastFiveDays_ReturnsOk_WhenDataExists()
        {
            var usage = new List<DTOs.DailyWaterUsageDTO>
            {
                new DTOs.DailyWaterUsageDTO(),
                new DTOs.DailyWaterUsageDTO()
            };
            _mockLogLogic.Setup(x => x.GetWaterUsageForLastFiveDaysAsync(1)).ReturnsAsync(usage);

            var result = await _controller.GetWaterUsageForLastFiveDays(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            if (result.Result is OkObjectResult okResult)
            {
                Assert.That(okResult.Value, Is.EqualTo(usage));
            }
            else
            {
                Assert.Fail("Result is not OkObjectResult");
            }
        }

        [Test]
        public async Task GetWaterUsageForLastFiveDays_ReturnsNotFound_WhenNoData()
        {
            _mockLogLogic
                .Setup(x => x.GetWaterUsageForLastFiveDaysAsync(1))
                .ReturnsAsync((List<DTOs.DailyWaterUsageDTO>)null!);

            var result = await _controller.GetWaterUsageForLastFiveDays(1);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            if (result.Result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(
                    notFoundResult.Value,
                    Is.EqualTo("No water usage data found for greenhouse with id 1")
                );
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }

        [Test]
        public async Task GetWaterUsageForLastFiveDays_ReturnsNotFound_WhenEmptyList()
        {
            _mockLogLogic
                .Setup(x => x.GetWaterUsageForLastFiveDaysAsync(1))
                .ReturnsAsync(new List<DTOs.DailyWaterUsageDTO>());

            var result = await _controller.GetWaterUsageForLastFiveDays(1);

            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            if (result.Result is NotFoundObjectResult notFoundResult)
            {
                Assert.That(
                    notFoundResult.Value,
                    Is.EqualTo("No water usage data found for greenhouse with id 1")
                );
            }
            else
            {
                Assert.Fail("Result is not NotFoundObjectResult");
            }
        }
    }
}
