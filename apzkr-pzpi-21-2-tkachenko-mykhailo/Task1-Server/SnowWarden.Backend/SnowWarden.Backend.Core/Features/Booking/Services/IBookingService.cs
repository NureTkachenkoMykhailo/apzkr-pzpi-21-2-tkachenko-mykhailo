using SnowWarden.Backend.Core.Abstractions;

namespace SnowWarden.Backend.Core.Features.Booking.Services;

public interface IBookingService : IBasicDataAccessService<Booking>
{
	Task<List<Booking>> GetUserBookings(int userId);
	Task CancelBooking(Booking booking);
}