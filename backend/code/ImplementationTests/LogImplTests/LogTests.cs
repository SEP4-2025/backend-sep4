using Database;
using Entities;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.LogImplTests;

public class LogTests
{
    private AppDbContext _context;
    private ILogInterface _logLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _logLogic = new LogLogic(_context);
    }

    [Test]
    public async Task GetLogByIdAsync_Success_ReturnsCorrectLog()
    {
        var testLog = await LogSeeder.SeedLogAsync();

        var result = await _logLogic.GetLogByIdAsync(testLog.Id);

        Assert.IsNotNull(result);
        Assert.That(result.Message, Is.EqualTo(testLog.Message));
    }

    [Test]
    public void GetLogByIdAsync_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _logLogic.GetLogByIdAsync(9999)
        );
        Assert.That(exception.Message, Is.EqualTo("Log not found."));
    }

    [Test]
    public async Task GetLogsByDateAsync_Success_ReturnsCorrectLogs()
    {
        var today = DateTime.Today;
        var testLog = await LogSeeder.SeedLogAsync();

        var result = await _logLogic.GetLogsByDateAsync(today);

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(testLog.Id));
    }

    [Test]
    public async Task GetAllLogs_Success_ReturnsAllLogs()
    {
        await LogSeeder.SeedLogAsync();
        await LogSeeder.SeedLogAsync();

        var result = await _logLogic.GetAllLogs();

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteLogAsync_Success_DeletesLog()
    {
        var testLog = await LogSeeder.SeedLogAsync();

        await _logLogic.DeleteLogAsync(testLog.Id);
        var logs = await _logLogic.GetAllLogs();

        Assert.IsEmpty(logs);
    }

    [Test]
    public async Task GetWaterUsageForLastFiveDaysAsync_ReturnsEmpty_WhenNoLogs()
    {
        var result = await _logLogic.GetWaterUsageForLastFiveDaysAsync(1);

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetWaterUsageForLastFiveDaysAsync_ReturnsCorrectUsage()
    {
        var log = await LogSeeder.SeedLogWithMessageAsync("Plant watered with amount: 150.");
        var log2 = await LogSeeder.SeedLogWithMessageAsync("Water tank refilled to 200.");

        log.GreenhouseId = 1;
        log2.GreenhouseId = 1;
        await _context.SaveChangesAsync();

        var result = await _logLogic.GetWaterUsageForLastFiveDaysAsync(1);

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Logs.RemoveRange(_context.Logs.ToList());
        _context.SaveChanges();
    }
}
