using Database;
using DTOs;
using Entities;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.SensorImplTests;

public class SensorTests
{
    private AppDbContext _context;
    private ISensorInterface _sensorLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _sensorLogic = new SensorLogic(_context);
    }

    [Test]
    public async Task GetSensorByIdAsync_Success_ReturnsCorrectSensor()
    {
        var testSensor = await SensorSeeder.SeedSensorAsync();

        var result = await _sensorLogic.GetSensorByIdAsync(testSensor.Id);

        Assert.IsNotNull(result);
        Assert.That(result.Type, Is.EqualTo(testSensor.Type));
        Assert.That(result.MetricUnit, Is.EqualTo(testSensor.MetricUnit));
        Assert.That(result.GreenhouseId, Is.EqualTo(testSensor.GreenhouseId));
    }

    [Test]
    public async Task GetAllSensorsAsync_Success_ReturnsAllSensors()
    {
        await SensorSeeder.SeedSensorAsync();
        await SensorSeeder.SeedSensorAsync(greenhouseId: 2);

        var result = await _sensorLogic.GetAllSensorsAsync();

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task AddSensorAsync_Success_AddsSensorCorrectly()
    {
        var sensorDto = new AddSensorDTO
        {
            Type = "Humidity",
            MetricUnit = "Percentage",
            GreenHouseId = 3
        };

        var result = await _sensorLogic.AddSensorAsync(sensorDto);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Type, Is.EqualTo(sensorDto.Type));
        Assert.That(result.MetricUnit, Is.EqualTo(sensorDto.MetricUnit));
        Assert.That(result.GreenhouseId, Is.EqualTo(sensorDto.GreenHouseId));
    }

    [Test]
    public async Task UpdateSensorAsync_Success_UpdatesSensorCorrectly()
    {
        var testSensor = await SensorSeeder.SeedSensorAsync();
        var updatedSensor = new UpdateSensorDTO()
        {
            Type = "Light",
            MetricUnit = "Lux",
        };

        var result = await _sensorLogic.UpdateSensorAsync(testSensor.Id, updatedSensor);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(testSensor.Id));
        Assert.That(result.Type, Is.EqualTo(updatedSensor.Type));
        Assert.That(result.MetricUnit, Is.EqualTo(updatedSensor.MetricUnit));
    }

    [Test]
    public async Task DeleteSensorAsync_Success_DeletesSensor()
    {
        var testSensor = await SensorSeeder.SeedSensorAsync();

        await _sensorLogic.DeleteSensorAsync(testSensor.Id);

        var result = await _sensorLogic.GetSensorByIdAsync(testSensor.Id);
        Assert.IsNull(result);
    }

    [Test]
    public async Task UpdateSensorAsync_NonExistentId_ThrowsException()
    {
        var nonExistentSensor = new Sensor
        {
            Id = 9999,
            Type = "Soil Moisture",
            MetricUnit = "Percentage",
            GreenhouseId = 1
        };

        Assert.ThrowsAsync<NullReferenceException>(async () =>
            await _sensorLogic.UpdateSensorAsync(
                nonExistentSensor.Id,
                new UpdateSensorDTO
                {
                    Type = nonExistentSensor.Type,
                    MetricUnit = nonExistentSensor.MetricUnit
                }));
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.Sensors.RemoveRange(_context.Sensors);
        await _context.SaveChangesAsync();
    }
}