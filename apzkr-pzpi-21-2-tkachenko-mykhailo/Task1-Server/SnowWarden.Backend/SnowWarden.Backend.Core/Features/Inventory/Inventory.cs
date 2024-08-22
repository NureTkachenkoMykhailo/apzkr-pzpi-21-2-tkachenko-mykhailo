using SnowWarden.Backend.Core.Abstractions;

namespace SnowWarden.Backend.Core.Features.Inventory;

public class Inventory : IDbEntity
{
	public int Id { get; private set; }
	public string Name { get; set; }

	public ICollection<InventoryItem>? InventoryItems { get; set; }
	public void SetExistingId(int id) => Id = id;
}