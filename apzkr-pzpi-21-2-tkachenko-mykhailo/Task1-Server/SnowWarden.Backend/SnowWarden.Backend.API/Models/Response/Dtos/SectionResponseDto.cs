using SnowWarden.Backend.Core.Features.Track;

namespace SnowWarden.Backend.API.Models.Response.Dtos;

public class SectionResponseDto
{
	public int Id { get; set; }
	public int TrackId { get; set; }
	public SectionInfo Information { get; set; }
	public Dictionary<string, SectionFactor> Factors { get; set; }
	public int AggregateDangerIndex { get; set; }
}