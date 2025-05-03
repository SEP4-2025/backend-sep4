using Database;
using Entities;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.PredictionImplTests;

public class PredictionTests
{
    private AppDbContext _context;
    private IPredictionInterface _predictionLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _predictionLogic = new PredictionLogic(_context);
    }

    [Test]
    public async Task GetPredictionByIdAsync_Success_ReturnsCorrectPrediction()
    {
        var testPrediction = await PredictionSeeder.SeedPredictionAsync();

        var result = await _predictionLogic.GetPredictionByIdAsync(testPrediction.Id);

        Assert.IsNotNull(result);
        Assert.That(result.OptimalTemperature, Is.EqualTo(testPrediction.OptimalTemperature));
        Assert.That(result.OptimalHumidity, Is.EqualTo(testPrediction.OptimalHumidity));
        Assert.That(result.OptimalLight, Is.EqualTo(testPrediction.OptimalLight));
        Assert.That(result.OptimalWaterLevel, Is.EqualTo(testPrediction.OptimalWaterLevel));
        Assert.That(result.GreenhouseId, Is.EqualTo(testPrediction.GreenhouseId));
        Assert.That(result.SensorReadingId, Is.EqualTo(testPrediction.SensorReadingId));
    }

    [Test]
    public async Task GetPredictionsByDateAsync_Success_ReturnsCorrectPredictions()
    {
        var date = DateTime.Now.Date;
        await PredictionSeeder.SeedPredictionAsync();
        await PredictionSeeder.SeedPredictionAsync();

        var result = await _predictionLogic.GetPredictionsByDateAsync(date);

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(p => p.Date.Date == date), Is.True);
    }

    [Test]
    public async Task GetAllPredictions_Success_ReturnsAllPredictions()
    {
        await PredictionSeeder.SeedPredictionAsync();
        await PredictionSeeder.SeedPredictionAsync();

        var result = await _predictionLogic.GetAllPredictions();

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task AddPredictionAsync_Success_AddsPredictionCorrectly()
    {
        var prediction = new Prediction
        {
            OptimalTemperature = 22,
            OptimalHumidity = 55,
            OptimalLight = 65,
            OptimalWaterLevel = 70,
            Date = DateTime.Now,
            GreenhouseId = 2,
            SensorReadingId = 2
        };

        await _predictionLogic.AddPredictionAsync(prediction);

        var allPredictions = await _predictionLogic.GetAllPredictions();
        var addedPrediction = allPredictions.FirstOrDefault(p =>
            p.OptimalTemperature == prediction.OptimalTemperature &&
            p.OptimalHumidity == prediction.OptimalHumidity &&
            p.GreenhouseId == prediction.GreenhouseId &&
            p.SensorReadingId == prediction.SensorReadingId);

        Assert.IsNotNull(addedPrediction);
        Assert.That(addedPrediction.OptimalTemperature, Is.EqualTo(prediction.OptimalTemperature));
        Assert.That(addedPrediction.OptimalHumidity, Is.EqualTo(prediction.OptimalHumidity));
        Assert.That(addedPrediction.OptimalLight, Is.EqualTo(prediction.OptimalLight));
        Assert.That(addedPrediction.OptimalWaterLevel, Is.EqualTo(prediction.OptimalWaterLevel));
        Assert.That(addedPrediction.GreenhouseId, Is.EqualTo(prediction.GreenhouseId));
        Assert.That(addedPrediction.SensorReadingId, Is.EqualTo(prediction.SensorReadingId));
    }

    [Test]
    public async Task DeletePredictionAsync_Success_DeletesPrediction()
    {
        var testPrediction = await PredictionSeeder.SeedPredictionAsync();

        await _predictionLogic.DeletePredictionAsync(testPrediction.Id);

        var result = await _predictionLogic.GetPredictionByIdAsync(testPrediction.Id);
        Assert.IsNull(result);
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.Predictions.RemoveRange(_context.Predictions);
        await _context.SaveChangesAsync();
    }
}