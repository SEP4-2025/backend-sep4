using Entities;

namespace ImplementationTests.LogImplTests;

public static class LogSeeder
{
    public static async Task<Log> SeedLogAsync(int sensorId = 1, int waterPumpId = 1)
    {
        var context = TestSetup.Context;

        var log = new Log
        {
            Timestamp = DateTime.Today,
            SensorReadingId = sensorId,
            WaterPumpId = waterPumpId,
            Message = "Test log message"
        };

        await context.Logs.AddAsync(log);
        await context.SaveChangesAsync();

        return log;
    }

    public static async Task<Log> SeedLogWithMessageAsync(string message, int sensorId = 1, int waterPumpId = 1)
    {
        var context = TestSetup.Context;
        var log = new Log
        {
            Timestamp = DateTime.Today,
            SensorReadingId = sensorId,
            WaterPumpId = waterPumpId,
            Message = message
        };
        await context.Logs.AddAsync(log);
        await context.SaveChangesAsync();
        return log;
    }
}