using Entities;

namespace ImplementationTests.PredictionImplTests;

public static class PredictionSeeder
{
    public static async Task<Prediction> SeedPredictionAsync(
        int greenhouseId = 1,
        int sensorReadingId = 4,
        DateTime? date = null
    )
    {
        var context = TestSetup.Context;

          var prediction = new Prediction
        {
            Temperature = 25,
            AirHumidity = 60,
            Light = 75,
            SoilHumidity = 80,
            Date = date ?? DateTime.UtcNow,
            GreenhouseId = greenhouseId,
            SensorReadingId = sensorReadingId
        };


        await context.Predictions.AddAsync(prediction);
        await context.SaveChangesAsync();

        return prediction;
    }
}