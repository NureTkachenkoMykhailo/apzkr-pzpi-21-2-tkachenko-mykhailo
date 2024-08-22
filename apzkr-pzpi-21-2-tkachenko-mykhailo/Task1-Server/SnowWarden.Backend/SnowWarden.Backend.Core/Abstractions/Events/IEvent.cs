using MediatR;

namespace SnowWarden.Backend.Core.Abstractions.Events;

public interface IEvent : INotification
{
	public Guid EventId { get; init; }
	public DateTime NextOnFail { get; }
	public int TimeoutSeconds { get; }

	public void ScheduleNext();
}