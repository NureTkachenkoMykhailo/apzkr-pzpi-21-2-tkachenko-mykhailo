using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Features.Training;

namespace SnowWarden.Backend.Core.Features.Inventory.Services;

public interface IInventoryItemService : IBasicDataAccessService<InventoryItem>
{
	Task<TrainingSession?> IsReservedForRequestedTime(int id, DateTime reservationTime, int duration);
}