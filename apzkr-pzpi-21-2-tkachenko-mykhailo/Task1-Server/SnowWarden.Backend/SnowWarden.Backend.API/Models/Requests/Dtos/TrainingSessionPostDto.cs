using SnowWarden.Backend.Core.Features.Training;

namespace SnowWarden.Backend.API.Models.Requests.Dtos;

public class TrainingSessionPostDto
{
	public int TrackId { get; set; }
	public int InstructorId { get; set; }
	public ICollection<TrainingLevel> Levels { get; set; }
	public TrainingInformation Information { get; set; }
}