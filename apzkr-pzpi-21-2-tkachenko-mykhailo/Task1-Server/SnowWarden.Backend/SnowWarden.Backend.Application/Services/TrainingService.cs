using SnowWarden.Backend.Core.Abstractions;

using SnowWarden.Backend.Core.Features.Training;
using SnowWarden.Backend.Core.Features.Training.Services;

namespace SnowWarden.Backend.Application.Services;

public class TrainingService(IRepository<TrainingSession> trainingRepository) : ServiceBase<TrainingSession>(trainingRepository), ITrainingService
{
	// public async Task<IReadOnlyCollection<TrainingSession>> GetReadonlyLightweightAsync() =>
	// 	await trainingRepository.GetReadonlyLightweightAsync();
	//
	// public Task<IReadOnlyCollection<TrainingSession>> GetReadonlyLightweightAsync(Expression<Func<TrainingSession, bool>> predicate)
	// {
	// 	throw new NotImplementedException();
	// }
	//
	// public Task<TrainingSession?> GetByIdCompleteAsync(int id) => trainingRepository.GetByIdCompleteAsync(id);
	// public Task<IReadOnlyCollection<TrainingSession>> GetReadonlyCompleteAsync(Expression<Func<TrainingSession, bool>> predicate)
	// {
	// 	throw new NotImplementedException();
	// }
	//
	// public async Task<IReadOnlyCollection<TrainingSession>> GetReadonlyCompleteAsync() =>
	// 	await trainingRepository.GetReadonlyCompleteAsync();
	//
	// public async Task<List<TrainingSession>> GetCompleteAsync() => await trainingRepository.GetCompleteAsync();
	// public async Task<List<TrainingSession>> GetLightweightAsync() => await trainingRepository.GetLightweightAsync();
	//
	// public async Task<TrainingSession> CreateAsync(TrainingSession trainingSession) => await trainingRepository.CreateAsync(trainingSession);
	// public async Task<TrainingSession> UpdateAsync(TrainingSession trainingSession) => await trainingRepository.UpdateAsync(trainingSession);
	// public async Task<TrainingSession> DeleteAsync(TrainingSession trainingSession) => await trainingRepository.DeleteAsync(trainingSession);
}