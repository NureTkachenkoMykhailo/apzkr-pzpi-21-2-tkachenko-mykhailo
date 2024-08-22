namespace SnowWarden.Backend.API.Models.Response.Dtos;

public class BookingResponseDto
{
	public int Id { get; private set; }

	public int TrainingId { get; set; }
	public int GuestId { get; set; }

	public UserResponseDto Guest { get; set; }
	public TrainingSessionResponseDto Training { get; set; }
}