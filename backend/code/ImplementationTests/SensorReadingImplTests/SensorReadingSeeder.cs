using Entities;
using DTOs;

namespace ImplementationTests.SensorReadingImplTests;

public static class SensorReadingSeeder
{
    public static async Task<SensorReading> SeedSensorReadingAsync(int sensorId = 1)
    {
        var context = TestSetup.Context;

        var sensorReading = new SensorReading
        {
            Value = 25,
            TimeStamp = DateTime.Today,
            SensorId = sensorId
        };

        await context.SensorReadings.AddAsync(sensorReading);
        await context.SaveChangesAsync();

        return sensorReading;
    }
}