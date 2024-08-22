using System.Linq.Expressions;
using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Core.Features.Track.Services;

namespace SnowWarden.Backend.Application.Services;

public class TrackSectionService(IRepository<Section> repository) : ServiceBase<Section>(repository), ITrackSectionService
{
	// public Task<IReadOnlyCollection<Section>> GetReadonlyLightweightAsync() => repository.GetReadonlyLightweightAsync();
	//
	// public Task<IReadOnlyCollection<Section>> GetReadonlyLightweightAsync(Expression<Func<Section, bool>> predicate) =>
	// 	repository.GetReadonlyLightweightAsync(predicate);
	//
	// public Task<Section?> GetByIdCompleteAsync(int id) => repository.GetByIdCompleteAsync(id);
	//
	// public Task<IReadOnlyCollection<Section>> GetReadonlyCompleteAsync(Expression<Func<Section, bool>> predicate) =>
	// 	repository.GetReadonlyCompleteAsync(predicate);
	//
	// public Task<IReadOnlyCollection<Section>> GetReadonlyCompleteAsync() => repository.GetReadonlyCompleteAsync();
	//
	// public async Task<List<Section>> GetCompleteAsync() => await repository.GetCompleteAsync();
	// public async Task<List<Section>> GetLightweightAsync() => await repository.GetLightweightAsync();
	//
	// public async Task<Section> CreateAsync(Section section) => await repository.CreateAsync(section);
	//
	// public Task<ICollection<Section>> CreateRange(params Section[] entities) => repository.CreateRange(entities);
	// public async Task<Section> UpdateAsync(Section section) => await repository.UpdateAsync(section);
	// public async Task<Section> DeleteAsync(Section section) => await repository.DeleteAsync(section);
}