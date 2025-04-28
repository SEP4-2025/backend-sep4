using DTOs;
using Entities;

namespace ImplementationTests.GardenerImplementationTests;

public static class GardenerSeeder
{
    public static async Task<Gardener> SeedGardenerAsync()
    {
        var context = TestSetup.Context;

        var gardener = new Gardener
        {
            Username = "testGardener",
            Password = "testPassword"
        };

        await context.Gardeners.AddAsync(gardener);
        await context.SaveChangesAsync();

        return gardener;
    }

    public static GardenerDTO SeedGardenerDto()
    {
        var gardenerDto = new GardenerDTO
        {
            Username = "testGardener",
            Password = "testPassword"
        };

        return gardenerDto;
    }
}