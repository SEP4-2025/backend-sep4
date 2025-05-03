using Entities;
using DTOs;

namespace ImplementationTests.SensorImplTests;

public static class SensorSeeder
{
    public static async Task<Sensor> SeedSensorAsync(int greenhouseId = 1)
    {
        var context = TestSetup.Context;

        var sensor = new Sensor
        {
            Type = "Temperature",
            MetricUnit = "Celsius",
            GreenhouseId = greenhouseId
        };

        await context.Sensors.AddAsync(sensor);
        await context.SaveChangesAsync();

        return sensor;
    }
}