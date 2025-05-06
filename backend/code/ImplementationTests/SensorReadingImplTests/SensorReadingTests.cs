using Database;
using DTOs;
using Entities;
using LogicImplements;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

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
    public async Task GetSensorReadingsByDateAsync_Success_ReturnsCorrectReadings()
    {
        var date = DateTime.Now;
        await SensorReadingSeeder.SeedSensorReadingAsync();
        await SensorReadingSeeder.SeedSensorReadingAsync();

        var result = await _sensorReadingLogic.GetSensorReadingsByDateAsync(DateTime.Today);

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(2));
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
    public async Task DeleteSensorReadingAsync_Success_DeletesReading()
    {
        var testReading = await SensorReadingSeeder.SeedSensorReadingAsync();

        await _sensorReadingLogic.DeleteSensorReadingAsync(testReading.Id);

        var result = await _sensorReadingLogic.GetSensorReadingByIdAsync(testReading.Id);
        Assert.IsNull(result);
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.SensorReadings.RemoveRange(_context.SensorReadings);
        await _context.SaveChangesAsync();
    }
}