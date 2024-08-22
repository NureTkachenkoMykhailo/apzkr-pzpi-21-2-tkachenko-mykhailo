using System.Linq.Expressions;
using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Features.Booking;
using SnowWarden.Backend.Core.Features.Booking.Services;

namespace SnowWarden.Backend.Application.Services;

public class BookingService(IRepository<Booking> repository) : ServiceBase<Booking>(repository), IBookingService
{
	private readonly IRepository<Booking> _repository = repository;

	public override Task<Booking> CreateAsync(Booking booking)
	{
		booking.Create();
		return base.CreateAsync(booking);
	}

	public override Task<ICollection<Booking>> CreateRange(params Booking[] bookings)
	{
		foreach (Booking booking in bookings)
		{
			booking.Create();
		}

		return base.CreateRange(bookings);
	}

	public Task<List<Booking>> GetUserBookings(int userId) =>
		_repository.GetCompleteAsync(b => b.GuestId == userId);

	public Task<IReadOnlyCollection<Booking>> GetReadonlyUserBookings(int userId) =>
		_repository.GetReadonlyCompleteAsync(b => b.GuestId == userId);

	public Task CancelBooking(Booking booking)
	{
		booking.Cancel();
		return UpdateAsync(booking);
	}
}