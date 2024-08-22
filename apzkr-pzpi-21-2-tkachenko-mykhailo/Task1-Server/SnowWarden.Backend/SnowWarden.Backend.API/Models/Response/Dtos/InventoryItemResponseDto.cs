using SnowWarden.Backend.Core.Features.Inventory;

namespace SnowWarden.Backend.API.Models.Response.Dtos;

public class InventoryItemResponseDto
{
	public int Id { get; private set; }
	public int InventoryId { get; set; }
	public required string Name { get; set; }
	public ICollection<InventoryAttribute>? Attributes { get; set; }
}