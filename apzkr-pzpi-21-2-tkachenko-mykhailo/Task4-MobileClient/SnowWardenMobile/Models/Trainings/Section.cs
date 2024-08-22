namespace SnowWardenMobile.Models.Trainings;

public class Section
{
	public int Id { get; set; }
	public int TrackId { get; set; }
	public SectionInfo Information { get; set; }
	public int AggregateDangerIndex { get; set; }
}