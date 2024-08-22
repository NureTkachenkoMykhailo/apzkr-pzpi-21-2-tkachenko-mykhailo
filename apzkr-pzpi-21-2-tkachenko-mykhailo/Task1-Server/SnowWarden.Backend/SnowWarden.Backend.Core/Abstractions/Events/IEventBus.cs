namespace SnowWarden.Backend.Core.Abstractions.Events;

public interface IEventBus
{
	Task PublishAsync<TEvent>(
		TEvent @event,
		CancellationToken cancellationToken = default)
		where TEvent : class, IEvent;
}