using DTOs;
using Entities;

namespace ImplementationTests.GreenhouseImplTests;

public static class GreenhouseSeeder
{
    public static async Task<Greenhouse> SeedGreenhouseAsync(int gardenerId = 0)
    {
        var context = TestSetup.Context;

        var greenhouse = new Greenhouse
        {
            Name = "Test Greenhouse",
            GardenerId = gardenerId
        };

        await context.Greenhouses.AddAsync(greenhouse);
        await context.SaveChangesAsync();

        return greenhouse;
    }

    public static GreenhouseDTO SeedGreenhouseDto(int gardenerId = 0)
    {
        var greenhouseDto = new GreenhouseDTO
        {
            Name = "Test Greenhouse",
            GardenerId = gardenerId
        };

        return greenhouseDto;
    }
}