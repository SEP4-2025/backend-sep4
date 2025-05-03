using Entities;
using ImplementationTests.GreenhouseImplTests;

namespace ImplementationTests.PlantImpTests;

public static class PlantSeeder
{
    public static async Task<Plant> SeedPlantAsync()
    {
        var context = TestSetup.Context;

        var greenhouse = await GreenhouseSeeder.SeedGreenhouseAsync();

        var plant = new Plant
        {
            Name = "TestPlant",
            Species = "TestSpecies",
            GreenhouseId = greenhouse.Id,
        };

        await context.Plants.AddAsync(plant);
        await context.SaveChangesAsync();

        return plant;
    }
}