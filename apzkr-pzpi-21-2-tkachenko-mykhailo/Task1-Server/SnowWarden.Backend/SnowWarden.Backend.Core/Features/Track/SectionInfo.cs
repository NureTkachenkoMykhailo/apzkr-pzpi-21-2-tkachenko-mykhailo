namespace SnowWarden.Backend.Core.Features.Track;

public record SectionInfo(string Name, int CurvatureDegrees, DangerLevel Danger = DangerLevel.NoDanger)
{
	public string Name { get; set; } = Name;
	public int CurvatureDegrees { get; set; } = CurvatureDegrees;
	public DangerLevel Danger { get; set; } = Danger;
}