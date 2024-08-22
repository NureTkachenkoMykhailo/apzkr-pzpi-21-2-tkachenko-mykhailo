namespace SnowWarden.Backend.API.Models.Response.Dtos;

public class InventoryResponseDto
{
	public int Id { get; set; }
	public string Name { get; set; }

	public ICollection<InventoryItemResponseDto> InventoryItems { get; set; } = [];
}