using Database;
using DTOs;
using Entities;
using LogicInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tools;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LogicImplements;

public class PredictionLogic : IPredictionInterface
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpFactory;
    private readonly string _pythonBaseUrl;
    private readonly ILogger<PredictionLogic> _logger;

    public PredictionLogic(
        AppDbContext context,
        IHttpClientFactory httpFactory,
        IConfiguration config,
        ILogger<PredictionLogic> logger)
    {
        _context = context;
        _httpFactory = httpFactory;
        _logger = logger;
        _pythonBaseUrl = config["PythonApi:BaseUrl"]!.TrimEnd('/');
    }

    public async Task<Prediction?> GetPredictionByIdAsync(int id)
    {
        //return await _context.Predictions.FirstOrDefaultAsync(p => p.Id == id);
        throw new NotImplementedException("GetPredictionByIdAsync is not implemented.");
    }

    public async Task<List<Prediction>> GetPredictionsByDateAsync(DateTime date)
    {
        /*return await _context.Predictions
            .Where(p => p.Date.Date == date.Date)
            .ToListAsync();*/
        throw new NotImplementedException("GetPredictionsByDateAsync is not implemented.");
    }

    public async Task<List<Prediction>> GetAllPredictions()
    {
        //return await _context.Predictions.ToListAsync();
        throw new NotImplementedException("GetAllPredictions is not implemented.");
    }

    public async Task AddPredictionAsync(PredictionDTO prediction)
    {
        /*var newPrediction = new Prediction()
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

        //hardcoded because we do not handle greenhouseId correctly
        Logger.Log(1, $"New prediction added.");

        await _context.SaveChangesAsync();*/
        throw new NotImplementedException("AddPredictionAsync is not implemented.");
    }

    public async Task DeletePredictionAsync(int id)
    {
        /*var prediction = await _context.Predictions.FirstOrDefaultAsync(p => p.Id == id);
        if (prediction != null)
        {
            _context.Predictions.Remove(prediction);
            await _context.SaveChangesAsync();
        }*/
        throw new NotImplementedException("DeletePredictionAsync is not implemented.");
    }

    public async Task<Prediction> GetLastPredictionAsync()
    {
        var prediction = await _context.Predictions
            .OrderByDescending(p => p.Date)
            .FirstOrDefaultAsync();

        if (prediction == null)
            throw new InvalidOperationException("No predictions in database.");

        return prediction;
    }

    public async Task<PredictionResponseDTO> RepredictLatestAsync()
    {
        // 1) load last Prediction from DB
        var last = await _context.Predictions
            .OrderByDescending(p => p.Date)
            .FirstOrDefaultAsync();
        if (last == null)
            throw new InvalidOperationException("No predictions in database.");


        // 2) map to request DTO
        var req = new PredictionRequestDTO
        {
            Temperature = last.Temperature,
            Light = last.Light,
            AirHumidity = last.AirHumidity,
            SoilHumidity = last.SoilHumidity,
            Date = last.Date,
            GreenhouseId = last.GreenhouseId,
            SensorReadingId = last.SensorReadingId
        };

        // 3) call Python API
        var client = _httpFactory.CreateClient();
        PredictionResponseDTO? responseDto;
        try
        {
            var response = await client.PostAsJsonAsync(
                $"{_pythonBaseUrl}/predict", req);
            response.EnsureSuccessStatusCode();
            responseDto = await response
                .Content
                .ReadFromJsonAsync<PredictionResponseDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Python ML API");
            throw;
        }

        if (responseDto == null)
            throw new InvalidOperationException("Empty response from ML API.");

        return responseDto;
    }
}
