namespace SnowWarden.Backend.Core.Abstractions.Events;

public abstract class DomainEvent : IDomainEvent
{
	public Guid EventId { get; init; } = Guid.NewGuid();
	public DateTime NextOnFail { get; private set; }
	public virtual int TimeoutSeconds { get; } = 5;

	public void ScheduleNext()
	{
		NextOnFail = DateTime.UtcNow.AddSeconds(TimeoutSeconds);
	}
}