using SnowWarden.Backend.Core.Features.Training;

namespace SnowWarden.Backend.API.Models.Response.Dtos;

public class TrainingSessionResponseDto
{
	public int Id { get; private set; }
	public int TrackId { get; init; }
	public int InstructorId { get; init; }
	public ICollection<object> Levels { get; init; } = [];
	public TrainingInformation Information { get; init; }
	public TrackResponseDto Track { get; init; }
	public UserResponseDto Instructor { get; init; }
	public ICollection<InventoryItemResponseDto> ReservedItems { get; init; }
}