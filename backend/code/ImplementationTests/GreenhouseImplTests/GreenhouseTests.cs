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
    public async Task AddGreenhouseAsync_Success_AddsGreenhouseCorrectly()
    {
        var gardener = await GardenerSeeder.SeedGardenerAsync();
        var greenhouseDto = new GreenhouseDTO
        {
            Name = "New Greenhouse",
            GardenerId = gardener.Id
        };

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

    // Method not implemented in the controller
    // [Test]
    // public async Task UpdateGreenhouseAsync_Success_UpdatesCorrectly()
    // {
    //     // First create a gardener and greenhouse
    //     var oldGardener = await GardenerSeeder.SeedGardenerAsync();
    //     var oldGreenhouse = await GreenhouseSeeder.SeedGreenhouseAsync(oldGardener.Id);
    //
    //     // Create a new gardener for updating the greenhouse
    //     var newGardener = await GardenerSeeder.SeedGardenerAsync();
    //
    //     // Create updated greenhouse
    //     var updatedGreenhouse = new Greenhouse
    //     {
    //         Id = oldGreenhouse.Id,
    //         Name = "Updated Greenhouse Name",
    //         GardenerId = newGardener.Id
    //     };
    //
    //     var result = await _greenhouseLogic.(updatedGreenhouse);
    //
    //     // Verify update was successful
    //     Assert.IsNotNull(result);
    //     Assert.That(result.Name, Is.EqualTo(updatedGreenhouse.Name));
    //     Assert.That(result.GardenerId, Is.EqualTo(newGardener.Id));
    //     Assert.AreNotEqual(oldGreenhouse.Name, result.Name);
    // }

    [Test]
    public async Task DeleteGreenhouseAsync_Success_DeletesGreenhouse()
    {
        var testGreenhouse = await GreenhouseSeeder.SeedGreenhouseAsync();

        await _greenhouseLogic.DeleteGreenhouse(testGreenhouse.Id);

        var deleteGreenhose = await _greenhouseLogic.GetGreenhouseById(testGreenhouse.Id);

        Assert.That(deleteGreenhose, Is.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Greenhouses.RemoveRange(_context.Greenhouses.ToList());
        _context.SaveChanges();
    }
}