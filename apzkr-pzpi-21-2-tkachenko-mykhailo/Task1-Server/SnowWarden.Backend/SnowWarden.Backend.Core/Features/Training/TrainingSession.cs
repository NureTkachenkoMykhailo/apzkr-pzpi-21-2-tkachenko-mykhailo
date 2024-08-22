using SnowWarden.Backend.Core.Abstractions;

using SnowWarden.Backend.Core.Abstractions.Events;

using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Features.Training.Events;

namespace SnowWarden.Backend.Core.Features.Training;

public class TrainingSession : IDbEntity, IEventSource
{
	public int Id { get; private set; }
	public int TrackId { get; set; }
	public int InstructorId { get; set; }

	public ICollection<TrainingLevel> Levels { get; set; } = [];
	public TrainingInformation GeneralInformation { get; set; }
	public Track.Track Track { get; set; }
	public Instructor Instructor { get; set; }
	public ICollection<Booking.Booking>? Bookings { get; set; }
	public ICollection<InventoryItem>? InventoryItemsTook { get; set; }

	private readonly List<IDomainEvent> _domainEvents = [];

	public void RaiseTrainingHasCompletedEvent()
	{
		_domainEvents.Add(new TrainingCompletedEvent());
	}

	public IReadOnlyCollection<IDomainEvent> GetEvents()
	{
		List<IDomainEvent> events = _domainEvents.ToList();
		_domainEvents.Clear();
		return events;
	}

	public void SetExistingId(int id) => Id = id;
}