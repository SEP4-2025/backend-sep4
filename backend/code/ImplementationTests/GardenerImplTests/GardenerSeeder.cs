using DTOs;
using Entities;

namespace ImplementationTests.GardenerImplTests;

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
}