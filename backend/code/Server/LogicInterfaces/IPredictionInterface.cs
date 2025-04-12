using Entities;

namespace LogicInterfaces;

public interface IPredictionInterface
{
    Task<Prediction> GetPredictionByIdAsync(int id);
    Task<List<Prediction>> GetPredictionsByDateAsync(DateTime date);
    Task<List<Prediction>> GetAllPredictions();
    Task AddPredictionAsync(Prediction prediction);
    Task UpdatePredictionAsync(Prediction prediction);
    Task DeletePredictionAsync(int id);
}
