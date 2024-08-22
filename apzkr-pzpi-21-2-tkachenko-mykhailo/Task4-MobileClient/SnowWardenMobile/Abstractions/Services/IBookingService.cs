using SnowWardenMobile.Models;

namespace SnowWardenMobile.Abstractions.Services;

public interface IBookingService : IApiCallerService
{
	/// <summary>
	/// Create booking
	/// </summary>
	/// <param name="trainingId">Training id to attach booking to</param>
	/// <returns>Create booking object</returns>
	public Task<Booking> Create(int trainingId);


	/// <summary>
	/// Get user bookings
	/// </summary>
	/// <returns>Collection of booking objects</returns>
	Task<ICollection<Booking>> GetBookings();
}