using Entities;
using DTOs;

namespace ImplementationTests.WaterPumpImplTests;

public static class WaterPumpSeeder
{
    public static async Task<WaterPump> SeedWaterPumpAsync()
    {
        var context = TestSetup.Context;

        var waterPump = new WaterPump
        {
            LastWateredTime = DateTime.UtcNow.AddDays(-1),
            LastWaterAmount = 100,
            AutoWateringEnabled = true,
            WaterTankCapacity = 1000,
            WaterLevel = 800,
            ThresholdValue = 30
        };

        await context.WaterPumps.AddAsync(waterPump);
        await context.SaveChangesAsync();

        return waterPump;
    }
}