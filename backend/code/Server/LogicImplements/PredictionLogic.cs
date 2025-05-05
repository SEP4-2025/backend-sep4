using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LogicImplements;

public class PredictionLogic : IPredictionInterface
{
    public AppDbContext _context;

    public PredictionLogic(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Prediction?> GetPredictionByIdAsync(int id)
    {
        return await _context.Predictions.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Prediction>> GetPredictionsByDateAsync(DateTime date)
    {
        return await _context.Predictions
            .Where(p => p.Date.Date == date.Date)
            .ToListAsync();
    }

    public async Task<List<Prediction>> GetAllPredictions()
    {
        return await _context.Predictions.ToListAsync();
    }

    public async Task AddPredictionAsync(PredictionDTO prediction)
    {
        var newPrediction = new Prediction()
        {
            OptimalTemperature = prediction.OptimalTemperature,
            OptimalHumidity = prediction.OptimalHumidity,
            OptimalLight = prediction.OptimalLight,
            OptimalWaterLevel = prediction.OptimalWaterLevel,
            Date = DateTime.UtcNow,
            GreenhouseId = prediction.GreenhouseId,
            SensorReadingId = prediction.SensorReadingId,
        };

        await _context.Predictions.AddAsync(newPrediction);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePredictionAsync(int id)
    {
        var prediction = await _context.Predictions.FirstOrDefaultAsync(p => p.Id == id);
        if (prediction != null)
        {
            _context.Predictions.Remove(prediction);
            await _context.SaveChangesAsync();
        }
    }
}
