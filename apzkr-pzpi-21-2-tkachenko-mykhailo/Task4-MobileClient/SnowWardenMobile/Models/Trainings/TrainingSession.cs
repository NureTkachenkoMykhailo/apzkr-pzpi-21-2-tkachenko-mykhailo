namespace SnowWardenMobile.Models.Trainings;

public class TrainingSession
{
	public int Id { get; set; }
	public int TrackId { get; init; }
	public int InstructorId { get; init; }
	public ICollection<object> Levels { get; init; } = [];
	public TrainingInformation Information { get; init; }
	public Track Track { get; init; }
	public User Instructor { get; init; }
}