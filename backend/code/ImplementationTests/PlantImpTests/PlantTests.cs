using Database;
using DTOs;
using Entities;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.PlantImpTests;

public class PlantTests
{
    private AppDbContext _context;
    private IPlantInterface _plantLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _plantLogic = new PlantLogic(_context);
    }

    [Test]
    public async Task GetPlantByIdAsync_ReturnsCorrectPlant()
    {
        var seededPlant = await PlantSeeder.SeedPlantAsync();

        var result = await _plantLogic.GetPlantByIdAsync(seededPlant.Id);

        Assert.IsNotNull(result);
        Assert.That(result.Name, Is.EqualTo(seededPlant.Name));
    }

    [Test]
    public void GetPlantByIdAsync_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _plantLogic.GetPlantByIdAsync(-1)
        );
        Assert.That(exception.Message, Is.EqualTo("Plant not found."));
    }

    [Test]
    public async Task AddPlantAsync_AddsCorrectly()
    {
        var plant = new PlantDTO
        {
            Name = "NewPlant",
            Species = "NewSpecies",
            GreenhouseId = 1
        };

        var addedPlant = await _plantLogic.AddPlantAsync(plant);
        var fetched = await _plantLogic.GetPlantByIdAsync(addedPlant.Id);

        Assert.IsNotNull(fetched);
        Assert.That(fetched.Name, Is.EqualTo("NewPlant"));
    }

    [Test]
    public async Task UpdatePlantNameAsync_UpdatesNameCorrectly()
    {
        var plant = await PlantSeeder.SeedPlantAsync();

        var updated = await _plantLogic.UpdatePlantNameAsync(plant.Id, "UpdatedPlant");

        Assert.IsNotNull(updated);
        Assert.That(updated.Name, Is.EqualTo("UpdatedPlant"));
    }

    [Test]
    public void UpdatePlantNameAsync_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<NullReferenceException>(
            async () => await _plantLogic.UpdatePlantNameAsync(-1, "Name")
        );
        Assert.That(
            exception.Message,
            Is.EqualTo("Object reference not set to an instance of an object.")
        );
    }

    [Test]
    public async Task DeletePlantAsync_RemovesPlant()
    {
        var plant = await PlantSeeder.SeedPlantAsync();

        await _plantLogic.DeletePlantAsync(plant.Id);

        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _plantLogic.GetPlantByIdAsync(plant.Id)
        );
        Assert.That(exception.Message, Is.EqualTo("Plant not found."));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Plants.RemoveRange(_context.Plants.ToList());
        _context.SaveChanges();
    }
}
