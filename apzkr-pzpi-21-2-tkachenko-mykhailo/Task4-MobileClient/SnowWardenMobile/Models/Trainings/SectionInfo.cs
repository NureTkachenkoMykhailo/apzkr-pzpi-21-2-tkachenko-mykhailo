namespace SnowWardenMobile.Models.Trainings;

public record SectionInfo(string Name, int CurvatureDegrees, SectionInfo.DangerLevel Danger = SectionInfo.DangerLevel.NoDanger)
{
	public enum DangerLevel
	{
		NoDanger = 0,
		Mediocre = 10,
		Dangerous = 20,
		VeryDangerous = 30,
		ExceptionalDanger = 40
	}

	public string Name { get; set; } = Name;
	public int CurvatureDegrees { get; set; } = CurvatureDegrees;
	public DangerLevel Danger { get; set; } = Danger;
}