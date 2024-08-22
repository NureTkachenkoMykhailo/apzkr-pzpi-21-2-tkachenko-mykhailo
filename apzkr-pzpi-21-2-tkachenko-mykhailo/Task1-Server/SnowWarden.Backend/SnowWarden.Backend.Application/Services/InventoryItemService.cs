using SnowWarden.Backend.Application.Exceptions;

using SnowWarden.Backend.Core.Abstractions;

using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Inventory.Services;
using SnowWarden.Backend.Core.Features.Training;

using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Application.Services;

public class InventoryItemService(IRepository<InventoryItem> repository) : ServiceBase<InventoryItem>(repository), IInventoryItemService
{
	public async Task<TrainingSession?> IsReservedForRequestedTime(int id, DateTime reservationTime, int duration)
	{
		InventoryItem inventoryItem = await GetReadonlyByIdCompleteAsync(id) ?? throw new ServiceException(GetType().Name, new LocalizedContent
		{
			Translations =
			{
				{
					Localizator.SupportedLanguages.AmericanEnglish,
					$"Invalid id {id} for {nameof(InventoryItem)}"
				},
				{
					Localizator.SupportedLanguages.Ukrainian,
					$"Неправильний ідентифіктора {id} для {nameof(InventoryItem)}"
				}
			}
		});

		TrainingSession? reservedTraining = inventoryItem.TrainingSessions?
			.Where(ts =>
				ts.GeneralInformation.ApproximateFinishTime >= reservationTime.AddMinutes(duration))
			.MaxBy(ts => ts.GeneralInformation.ApproximateFinishTime);

		return reservedTraining;
	}

	// public Task<ICollection<InventoryItem>> CreateRange(params InventoryItem[] entities) =>
	// 	_repository.CreateRange(entities);
	//
	// public Task<InventoryItem> CreateAsync(InventoryItem entity) => _repository.CreateAsync(entity);
	//
	// public Task<InventoryItem> UpdateAsync(InventoryItem entity) => _repository.UpdateAsync(entity);
	//
	// public Task<InventoryItem> DeleteAsync(InventoryItem entity) => _repository.DeleteAsync(entity);
	//
	// public Task<InventoryItem?> GetByIdCompleteAsync(int id) => _repository.GetByIdCompleteAsync(id);
	//
	// public Task<IReadOnlyCollection<InventoryItem>> GetReadonlyCompleteAsync(Expression<Func<InventoryItem, bool>> predicate) =>
	// 	_repository.GetReadonlyCompleteAsync(predicate);
	//
	// public Task<IReadOnlyCollection<InventoryItem>> GetReadonlyCompleteAsync() => _repository.GetReadonlyCompleteAsync();
	//
	// public Task<IReadOnlyCollection<InventoryItem>> GetReadonlyLightweightAsync() =>
	// 	_repository.GetReadonlyLightweightAsync();
	//
	// public Task<IReadOnlyCollection<InventoryItem>> GetReadonlyLightweightAsync(Expression<Func<InventoryItem, bool>> predicate) =>
	// 	_repository.GetReadonlyLightweightAsync();
	//
	// public Task<List<InventoryItem>> GetCompleteAsync() => _repository.GetCompleteAsync();
	//
	// public Task<List<InventoryItem>> GetLightweightAsync() => _repository.GetLightweightAsync();
}