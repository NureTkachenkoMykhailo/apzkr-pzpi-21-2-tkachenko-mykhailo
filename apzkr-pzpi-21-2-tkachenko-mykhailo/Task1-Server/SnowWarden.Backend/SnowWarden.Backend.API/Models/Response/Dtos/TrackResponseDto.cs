namespace SnowWarden.Backend.API.Models.Response.Dtos;

public class TrackResponseDto
{
	public int Id { get; set; }

	public string Name { get; set; }

	public ICollection<SectionResponseDto> Sections { get; set; }
}