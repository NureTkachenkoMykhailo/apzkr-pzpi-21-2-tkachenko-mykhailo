using System.Text.Json.Serialization;

using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Features.Training;

namespace SnowWarden.Backend.Core.Features.Inventory;

public class InventoryItem : IDbEntity
{
	public int Id { get; private set; }
	public int InventoryId { get; set; }

	public required string Name { get; set; }

	public ICollection<InventoryAttribute>? Attributes { get; set; }
	[JsonIgnore] public Inventory Inventory { get; set; }
	[JsonIgnore] public ICollection<TrainingSession>? TrainingSessions { get; set; }

	public void SetExistingId(int id) => Id = id;
}