namespace SnowWardenMobile.Models.Trainings;

public class Track
{
	public int Id { get; set; }

	public string Name { get; set; }

	public ICollection<Section> Sections { get; set; }
}