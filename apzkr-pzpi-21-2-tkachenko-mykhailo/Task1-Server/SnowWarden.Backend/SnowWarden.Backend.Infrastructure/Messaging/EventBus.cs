using SnowWarden.Backend.Core.Abstractions.Events;

namespace SnowWarden.Backend.Infrastructure.Messaging;

public sealed class EventBus(EventChannel queue) : IEventBus
{
	public async Task PublishAsync<T>(
		T @event,
		CancellationToken cancellationToken = default)
		where T : class, IEvent
	{
		await queue.Writer.WriteAsync(@event, cancellationToken);
	}
}