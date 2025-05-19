using Database;
using DTOs;
using Entities;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.SensorReadingImplTests;

public class SensorReadingTests
{
    private AppDbContext _context;
    private ISensorReadingInterface _sensorReadingLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _sensorReadingLogic = new SensorReadingLogic(_context);
    }

    [Test]
    public Task GetSensorReadingByIdAsync_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _sensorReadingLogic.GetSensorReadingByIdAsync(-1)
        );
        Assert.That(exception.Message, Is.EqualTo("Sensor reading with id -1 not found."));
        return Task.CompletedTask;
    }

    [Test]
    public async Task GetSensorReadingByIdAsync_Success_ReturnsCorrectReading()
    {
        var testReading = await SensorReadingSeeder.SeedSensorReadingAsync();

        var result = await _sensorReadingLogic.GetSensorReadingByIdAsync(testReading.Id);

        Assert.IsNotNull(result);
        Assert.That(result.Value, Is.EqualTo(testReading.Value));
        Assert.That(result.SensorId, Is.EqualTo(testReading.SensorId));
    }

    [Test]
    public async Task GetSensorReadingsBySensorIdAsync_Success_ReturnsCorrectReadings()
    {
        // First, ensure a sensor exists
        var sensor = new Sensor
        {
            Type = "Temperature",
            MetricUnit = "Celsius",
            GreenhouseId = 1
        };
        await _context.Sensors.AddAsync(sensor);
        await _context.SaveChangesAsync();

        // Create two sensor readings for this sensor
        await SensorReadingSeeder.SeedSensorReadingAsync(sensor.Id);
        await SensorReadingSeeder.SeedSensorReadingAsync(sensor.Id);

        var result = await _sensorReadingLogic.GetSensorReadingsBySensorIdAsync(sensor.Id);

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(r => r.SensorId == sensor.Id), Is.True);

        // Clean up the sensor
        _context.Sensors.Remove(sensor);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task GetSensorReadingsAsync_Success_ReturnsAllReadings()
    {
        await SensorReadingSeeder.SeedSensorReadingAsync();
        await SensorReadingSeeder.SeedSensorReadingAsync();

        var result = await _sensorReadingLogic.GetSensorReadingsAsync();

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task AddSensorReadingAsync_Success_AddsReadingCorrectly()
    {
        var sensorReadingDto = new SensorReadingDTO
        {
            Value = 22,
            TimeStamp = DateTime.Now,
            SensorId = 1
        };

        var result = await _sensorReadingLogic.AddSensorReadingAsync(sensorReadingDto);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Value, Is.EqualTo(sensorReadingDto.Value));
        Assert.That(result.SensorId, Is.EqualTo(sensorReadingDto.SensorId));
    }

    [Test]
    public async Task DeleteSensorReadingAsync_RemovesReading()
    {
        var testReading = await SensorReadingSeeder.SeedSensorReadingAsync();

        await _sensorReadingLogic.DeleteSensorReadingAsync(testReading.Id);

        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _sensorReadingLogic.GetSensorReadingByIdAsync(testReading.Id)
        );
        Assert.That(
            exception.Message,
            Is.EqualTo($"Sensor reading with id {testReading.Id} not found.")
        );
    }

    [Test]
    public async Task GetAverageSensorReadingsFromLast24Hours_ReturnsEmpty_WhenNoReadings()
    {
        var result = await _sensorReadingLogic.GetAverageSensorReadingsFromLast24Hours(1);
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetAverageSensorReadingsFromLast24Hours_ReturnsCorrectAverage()
    {
        var sensor = new Sensor
        {
            Type = "Temperature",
            MetricUnit = "Celsius",
            GreenhouseId = 1
        };
        await _context.Sensors.AddAsync(sensor);
        await _context.SaveChangesAsync();
        await SensorReadingSeeder.SeedSensorReadingAsync(sensor.Id);
        var result = await _sensorReadingLogic.GetAverageSensorReadingsFromLast24Hours(1);
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task GetAverageReadingFromLast7Days_ReturnsEmpty_WhenNoReadings()
    {
        var result = await _sensorReadingLogic.GetAverageReadingFromLast7Days(1);
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetAverageReadingFromLast7Days_ReturnsCorrectAverage()
    {
        var sensor = new Sensor
        {
            Type = "Humidity",
            MetricUnit = "%",
            GreenhouseId = 1
        };
        await _context.Sensors.AddAsync(sensor);
        await _context.SaveChangesAsync();
        await SensorReadingSeeder.SeedSensorReadingAsync(sensor.Id);
        var result = await _sensorReadingLogic.GetAverageReadingFromLast7Days(1);
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task GetAverageReadingFromLast30Days_ReturnsEmpty_WhenNoReadings()
    {
        var result = await _sensorReadingLogic.GetAverageReadingFromLast30Days(1);
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetAverageReadingFromLast30Days_ReturnsCorrectAverage()
    {
        var sensor = new Sensor
        {
            Type = "Light",
            MetricUnit = "Lux",
            GreenhouseId = 1
        };
        await _context.Sensors.AddAsync(sensor);
        await _context.SaveChangesAsync();
        await SensorReadingSeeder.SeedSensorReadingAsync(sensor.Id);
        var result = await _sensorReadingLogic.GetAverageReadingFromLast30Days(1);
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.SensorReadings.RemoveRange(_context.SensorReadings);
        await _context.SaveChangesAsync();
    }
}
