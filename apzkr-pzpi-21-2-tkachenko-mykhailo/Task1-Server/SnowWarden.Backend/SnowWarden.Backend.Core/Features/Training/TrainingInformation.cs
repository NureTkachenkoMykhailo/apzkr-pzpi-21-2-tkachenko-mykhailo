namespace SnowWarden.Backend.Core.Features.Training;

public record TrainingInformation
{
	public string Name { get; set; }
	public string? Descriptions { get; set; }
	public int DurationMinutes { get; set; }
	public DateTime Start { get; set; }

	public DateTime ApproximateFinishTime => Start.AddMinutes(DurationMinutes);
}