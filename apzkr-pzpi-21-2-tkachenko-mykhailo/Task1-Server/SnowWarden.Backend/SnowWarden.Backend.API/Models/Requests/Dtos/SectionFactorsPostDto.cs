namespace SnowWarden.Backend.API.Models.Requests.Dtos;

public class SectionFactorsPostDto
{
	public double Wind { get; set; } = default;
	public double Snow { get; set; } = default;
	public double Iciness { get; set; } = default;
}