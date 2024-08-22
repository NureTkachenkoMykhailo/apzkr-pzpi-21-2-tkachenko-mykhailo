namespace SnowWarden.Backend.API.Models.Requests.Dtos;

public class TrackPostDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public ICollection<SectionPostDto> Sections { get; set; }
}