using SnowWardenMobile.Models.Trainings;

namespace SnowWardenMobile.Abstractions.Services;

public interface ITrainingSessionService : IApiCallerService
{
    public Task<ICollection<TrainingSession>> GetTrainingSessionsAsync();
}