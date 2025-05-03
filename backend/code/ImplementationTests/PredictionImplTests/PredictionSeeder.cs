using Entities;

namespace ImplementationTests.PredictionImplTests;

public static class PredictionSeeder
{
    public static async Task<Prediction> SeedPredictionAsync(int greenhouseId = 1, int sensorReadingId = 1)
    {
        var context = TestSetup.Context;

        var prediction = new Prediction
        {
            OptimalTemperature = 25,
            OptimalHumidity = 60,
            OptimalLight = 75,
            OptimalWaterLevel = 80,
            Date = DateTime.Now,
            GreenhouseId = greenhouseId,
            SensorReadingId = sensorReadingId
        };

        await context.Predictions.AddAsync(prediction);
        await context.SaveChangesAsync();

        return prediction;
    }
}