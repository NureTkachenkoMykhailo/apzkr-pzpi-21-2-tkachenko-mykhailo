using SnowWardenMobile.Models.Trainings;

namespace SnowWardenMobile.Models;

public class Booking
{
	public int Id { get; set; }

	public int TrainingId { get; set; }
	public int GuestId { get; set; }

	public User Guest { get; set; }
	public TrainingSession Training { get; set; }
}