using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Abstractions.Events;
using SnowWarden.Backend.Core.Features.Booking.Events;
using SnowWarden.Backend.Core.Features.Members;

namespace SnowWarden.Backend.Core.Features.Booking;

public class Booking : IDbEntity, IEventSource
{
	public int Id { get; private set; }

	public int TrainingId { get; set; }
	public int GuestId { get; set; }

	public Guest Guest { get; set; }
	public Training.TrainingSession TrainingSession { get; set; }

	public bool IsCancelled { get; private set; }
	public DateTime? CreatedAt { get; private set; }
	public DateTime CancelledAt { get; private set; }

	private readonly List<IDomainEvent> _events = [];

	private void RaiseBookingSuccessfulEvent() => _events.Add(new BookingSuccesfulEvent());
	private void RaiseBookingCancelledEvent() => _events.Add(new BookingCancelledEvent());

	public void Create()
	{
		if (
			CreatedAt is not null &&
			CreatedAt != default(DateTime))
			return;

		CreatedAt = DateTime.UtcNow;
		RaiseBookingSuccessfulEvent();
	}

	public void Cancel()
	{
		if (IsCancelled) return;

		IsCancelled = true;
		CancelledAt = DateTime.UtcNow;
		RaiseBookingCancelledEvent();
	}

	public IReadOnlyCollection<IDomainEvent> GetEvents() => _events;
	public void SetExistingId(int id) => Id = id;
}