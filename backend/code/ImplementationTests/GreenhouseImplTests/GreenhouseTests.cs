using Database;
using DTOs;
using Entities;
using ImplementationTests.GardenerImplTests;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.GreenhouseImplTests;

public class GreenhouseTests
{
    private AppDbContext _context;
    private IGreenhouseInterface _greenhouseLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _greenhouseLogic = new GreenhouseLogic(_context);
    }

    [Test]
    public async Task GetGreenhouseByIdAsync_Success_ReturnsCorrectGreenhouse()
    {
        var testGreenhouse = await GreenhouseSeeder.SeedGreenhouseAsync();

        var result = await _greenhouseLogic.GetGreenhouseById(testGreenhouse.Id);

        Assert.IsNotNull(result);
        Assert.That(result.Name, Is.EqualTo(testGreenhouse.Name));
    }

    [Test]
    public async Task GetGreenhouseByNameAsync_Success_ReturnsCorrectGreenhouse()
    {
        var testGreenhouse = await GreenhouseSeeder.SeedGreenhouseAsync();

        var result = await _greenhouseLogic.GetGreenhouseByName(testGreenhouse.Name);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(testGreenhouse.Id));
    }

    [Test]
    public async Task GetGreenhouseByGardenerIdAsync_Success_ReturnsCorrectGreenhouse()
    {
        var gardener = await GardenerSeeder.SeedGardenerAsync();
        var testGreenhouse = await GreenhouseSeeder.SeedGreenhouseAsync(gardener.Id);

        var result = await _greenhouseLogic.GetGreenhouseByGardenerId(gardener.Id);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(testGreenhouse.Id));
    }

    [Test]
    public void GetGreenhouseById_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _greenhouseLogic.GetGreenhouseById(9999)
        );
        Assert.That(exception.Message, Is.EqualTo("Greenhouse with id 9999 not found."));
    }

    [Test]
    public void GetGreenhouseByName_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _greenhouseLogic.GetGreenhouseByName("doesnotexist")
        );
        Assert.That(exception.Message, Is.EqualTo("Greenhouse with name doesnotexist not found."));
    }

    [Test]
    public void GetGreenhouseByGardenerId_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _greenhouseLogic.GetGreenhouseByGardenerId(9999)
        );
        Assert.That(exception.Message, Is.EqualTo("Greenhouse with gardener id 9999 not found."));
    }

    [Test]
    public async Task AddGreenhouseAsync_Success_AddsGreenhouseCorrectly()
    {
        var gardener = await GardenerSeeder.SeedGardenerAsync();
        var greenhouseDto = new GreenhouseDTO { Name = "New Greenhouse", GardenerId = gardener.Id };

        var result = await _greenhouseLogic.AddGreenhouse(greenhouseDto);

        Assert.IsNotNull(result);
        Assert.That(result.Name, Is.EqualTo(greenhouseDto.Name));
        Assert.That(result.GardenerId, Is.EqualTo(gardener.Id));
    }

    [Test]
    public async Task UpdateGreenhouseName_Success_UpdatesNameCorrectly()
    {
        var testGreenhouse = await GreenhouseSeeder.SeedGreenhouseAsync();
        var newName = "Updated Greenhouse";

        var result = await _greenhouseLogic.UpdateGreenhouseName(testGreenhouse.Id, newName);

        Assert.IsNotNull(result);
        Assert.That(result.Name, Is.EqualTo(newName));
    }

    [Test]
    public void UpdateGreenhouseName_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _greenhouseLogic.UpdateGreenhouseName(9999, "newName")
        );
        Assert.That(exception.Message, Is.EqualTo("Greenhouse with id 9999 not found."));
    }

    [Test]
    public async Task UpdateGreenhouse_Success_UpdatesGreenhouseCorrectly()
    {
        var gardener = await GardenerSeeder.SeedGardenerAsync();
        var testGreenhouse = await GreenhouseSeeder.SeedGreenhouseAsync(gardener.Id);
        testGreenhouse.Name = "Updated Name";
        testGreenhouse.GardenerId = gardener.Id;
        var result = await _greenhouseLogic.UpdateGreenhouse(testGreenhouse);
        Assert.IsNotNull(result);
        Assert.That(result.Name, Is.EqualTo("Updated Name"));
        Assert.That(result.GardenerId, Is.EqualTo(gardener.Id));
    }

    [Test]
    public void UpdateGreenhouse_Throws_WhenNotFound()
    {
        var greenhouse = new Greenhouse
        {
            Id = -1,
            Name = "DoesNotExist",
            GardenerId = 1
        };
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _greenhouseLogic.UpdateGreenhouse(greenhouse)
        );
        Assert.That(exception.Message, Is.EqualTo("Greenhouse with id -1 not found."));
    }

    [Test]
    public async Task DeleteGreenhouseAsync_Success_DeletesGreenhouse()
    {
        var testGreenhouse = await GreenhouseSeeder.SeedGreenhouseAsync();

        await _greenhouseLogic.DeleteGreenhouse(testGreenhouse.Id);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _greenhouseLogic.GetGreenhouseById(testGreenhouse.Id)
        );
        Assert.That(ex.Message, Is.EqualTo($"Greenhouse with id {testGreenhouse.Id} not found."));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Greenhouses.RemoveRange(_context.Greenhouses.ToList());
        _context.SaveChanges();
    }
}
