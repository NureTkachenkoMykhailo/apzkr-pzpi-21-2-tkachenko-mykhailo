using SnowWarden.Backend.Core.Features.Track;

namespace SnowWarden.Backend.API.Models.Requests.Dtos;

public class SectionPostDto
{
	public int Id { get; private set; }
	public int TrackId { get; set; }
	public required SectionInfo Information { get; set; }
	public SectionFactorsPostDto Factors { get; set; } = new();
}