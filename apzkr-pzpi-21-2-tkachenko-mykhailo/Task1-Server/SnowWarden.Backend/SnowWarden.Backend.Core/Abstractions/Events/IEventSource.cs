namespace SnowWarden.Backend.Core.Abstractions.Events;

public interface IEventSource
{
	public IReadOnlyCollection<IDomainEvent> GetEvents();
}