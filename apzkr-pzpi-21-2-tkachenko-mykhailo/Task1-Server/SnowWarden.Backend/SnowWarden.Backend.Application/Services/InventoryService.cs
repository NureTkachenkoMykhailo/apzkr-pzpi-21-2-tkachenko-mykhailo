using System.Linq.Expressions;

using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Inventory.Services;

namespace SnowWarden.Backend.Application.Services;

public class InventoryService(IRepository<Inventory> repository) : ServiceBase<Inventory>(repository), IInventoryService
{
	// public Task<ICollection<Inventory>> CreateRange(params Inventory[] entities) => repository.CreateRange(entities);
	//
	// public Task<Inventory> CreateAsync(Inventory entity) => repository.CreateAsync(entity);
	//
	// public Task<Inventory> UpdateAsync(Inventory entity) => repository.UpdateAsync(entity);
	//
	// public Task<Inventory> DeleteAsync(Inventory entity) => repository.DeleteAsync(entity);
	//
	// public Task<Inventory?> GetByIdCompleteAsync(int id) => repository.GetByIdCompleteAsync(id);
	//
	// public Task<IReadOnlyCollection<Inventory>> GetReadonlyCompleteAsync(Expression<Func<Inventory, bool>> predicate) =>
	// 	repository.GetReadonlyCompleteAsync();
	//
	// public Task<IReadOnlyCollection<Inventory>> GetReadonlyCompleteAsync() => repository.GetReadonlyCompleteAsync();
	//
	// public Task<IReadOnlyCollection<Inventory>> GetReadonlyLightweightAsync() =>
	// 	repository.GetReadonlyLightweightAsync();
	//
	// public Task<IReadOnlyCollection<Inventory>> GetReadonlyLightweightAsync(Expression<Func<Inventory, bool>> predicate) =>
	// 	repository.GetReadonlyLightweightAsync();
	//
	// public Task<List<Inventory>> GetCompleteAsync() => repository.GetCompleteAsync();
	//
	// public Task<List<Inventory>> GetLightweightAsync() => repository.GetCompleteAsync();
}