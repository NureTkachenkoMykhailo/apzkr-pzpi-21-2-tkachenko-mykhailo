using System;

namespace SnowWardenMobile.Models.Trainings;

public class TrainingInformation
{
	public string Name { get; set; }
	public string? Descriptions { get; set; }
	public int DurationMinutes { get; set; }
	public DateTime Start { get; set; }

	public DateTime ApproximateFinishTime => Start.AddMinutes(DurationMinutes);
}