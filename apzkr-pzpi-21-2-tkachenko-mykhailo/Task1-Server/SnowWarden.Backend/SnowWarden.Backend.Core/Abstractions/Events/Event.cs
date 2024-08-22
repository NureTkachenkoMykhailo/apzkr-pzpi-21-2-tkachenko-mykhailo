namespace SnowWarden.Backend.Core.Abstractions.Events;

public abstract record Event : IEvent
{
	public Guid EventId { get; init; } = Guid.NewGuid();

	public DateTime NextOnFail { get; private set; }
	public int TimeoutSeconds { get; } = 5;

	public void ScheduleNext()
	{
		NextOnFail = NextOnFail.AddSeconds(TimeoutSeconds);
	}
}