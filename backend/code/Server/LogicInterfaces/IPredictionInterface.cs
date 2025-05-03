using DTOs;
using Entities;

namespace LogicInterfaces;

public interface IPredictionInterface
{
    Task<Prediction> GetPredictionByIdAsync(int id);
    Task<List<Prediction>> GetPredictionsByDateAsync(DateTime date);
    Task<List<Prediction>> GetAllPredictions();
    Task AddPredictionAsync(PredictionDTO prediction);
    Task DeletePredictionAsync(int id);
}
