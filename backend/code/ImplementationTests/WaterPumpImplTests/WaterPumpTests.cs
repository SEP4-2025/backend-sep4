using Database;
using DTOs;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.WaterPumpImplTests;

public class WaterPumpTests
{
    private AppDbContext _context;
    private IWaterPumpInterface _waterPumpLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _waterPumpLogic = new WaterPumpLogic(_context);
    }

    [Test]
    public async Task GetWaterPumpByIdAsync_Success_ReturnsCorrectWaterPump()
    {
        var testWaterPump = await WaterPumpSeeder.SeedWaterPumpAsync();

        var result = await _waterPumpLogic.GetWaterPumpByIdAsync(testWaterPump.Id);

        Assert.IsNotNull(result);
        Assert.That(result.WaterLevel, Is.EqualTo(testWaterPump.WaterLevel));
        Assert.That(result.WaterTankCapacity, Is.EqualTo(testWaterPump.WaterTankCapacity));
        Assert.That(result.AutoWateringEnabled, Is.EqualTo(testWaterPump.AutoWateringEnabled));
        Assert.That(result.ThresholdValue, Is.EqualTo(testWaterPump.ThresholdValue));
    }

    [Test]
    public async Task GetAllWaterPumpsAsync_Success_ReturnsAllWaterPumps()
    {
        await WaterPumpSeeder.SeedWaterPumpAsync();
        await WaterPumpSeeder.SeedWaterPumpAsync();

        var result = await _waterPumpLogic.GetAllWaterPumpsAsync();

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task AddWaterPumpAsync_Success_AddsWaterPumpCorrectly()
    {
        var waterPumpDto = new WaterPumpDTO
        {
            LastWatered = DateTime.UtcNow.AddHours(-5),
            LastWaterAmount = 50,
            AutoWatering = true,
            WaterTankCapacity = 1500,
            CurrentWaterLevel = 1200,
            ThresholdValue = 25
        };

        var result = await _waterPumpLogic.AddWaterPumpAsync(waterPumpDto);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.WaterLevel, Is.EqualTo(waterPumpDto.CurrentWaterLevel));
        Assert.That(result.WaterTankCapacity, Is.EqualTo(waterPumpDto.WaterTankCapacity));
        Assert.That(result.AutoWateringEnabled, Is.EqualTo(waterPumpDto.AutoWatering));
        Assert.That(result.ThresholdValue, Is.EqualTo(waterPumpDto.ThresholdValue));
        Assert.That(result.LastWaterAmount, Is.EqualTo(waterPumpDto.LastWaterAmount));
    }

    [Test]
    public async Task UpdateAutoWateringStatusAsync_Success_UpdatesStatusCorrectly()
    {
        var testWaterPump = await WaterPumpSeeder.SeedWaterPumpAsync();
        bool newStatus = !testWaterPump.AutoWateringEnabled;

        var result = await _waterPumpLogic.ToggleAutomationStatusAsync(testWaterPump.Id, newStatus);

        Assert.IsNotNull(result);
        Assert.That(result.AutoWateringEnabled, Is.EqualTo(newStatus));
    }

    [Test]
    public async Task TriggerManualWateringAsync_Success_UpdatesWaterLevelAndWateringInfo()
    {
        var testWaterPump = await WaterPumpSeeder.SeedWaterPumpAsync();
        int expectedWaterLevel = testWaterPump.WaterLevel - testWaterPump.ThresholdValue;

        var result = await _waterPumpLogic.TriggerManualWateringAsync(testWaterPump.Id);

        Assert.IsNotNull(result);
        Assert.That(result.WaterLevel, Is.EqualTo(expectedWaterLevel));
        Assert.That(result.LastWaterAmount, Is.EqualTo(testWaterPump.ThresholdValue));
        Assert.That(result.LastWateredTime.Date, Is.EqualTo(DateTime.UtcNow.Date));
    }

    [Test]
    public async Task TriggerManualWateringAsync_InsufficientWater_ReturnsNull()
    {
        var testWaterPump = await WaterPumpSeeder.SeedWaterPumpAsync();
        testWaterPump.WaterLevel = 0;
        var result = await _waterPumpLogic.TriggerManualWateringAsync(testWaterPump.Id);

        Assert.IsNull(result);
    }

    [Test]
    public async Task UpdateCurrentWaterLevelAsync_ExceedsTankCapacity_CapsAtCapacity()
    {
        var testWaterPump = await WaterPumpSeeder.SeedWaterPumpAsync();
        int addedWater = testWaterPump.WaterTankCapacity - testWaterPump.WaterLevel + 100; // More than needed to fill

        var result = await _waterPumpLogic.UpdateCurrentWaterLevelAsync(testWaterPump.Id, addedWater);

        Assert.IsNotNull(result);
        Assert.That(result.WaterLevel, Is.EqualTo(testWaterPump.WaterTankCapacity));
    }

    [Test]
    public async Task UpdateThresholdValueAsync_Success_UpdatesThresholdCorrectly()
    {
        var testWaterPump = await WaterPumpSeeder.SeedWaterPumpAsync();
        int newThreshold = 45;

        var result = await _waterPumpLogic.UpdateThresholdValueAsync(testWaterPump.Id, newThreshold);

        Assert.IsNotNull(result);
        Assert.That(result.ThresholdValue, Is.EqualTo(newThreshold));
    }

    [Test]
    public async Task DeleteWaterPumpAsync_Success_DeletesWaterPump()
    {
        var testWaterPump = await WaterPumpSeeder.SeedWaterPumpAsync();

        await _waterPumpLogic.DeleteWaterPumpAsync(testWaterPump.Id);

        var result = await _waterPumpLogic.GetWaterPumpByIdAsync(testWaterPump.Id);
        Assert.IsNull(result);
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.WaterPumps.RemoveRange(_context.WaterPumps);
        await _context.SaveChangesAsync();
    }
}
