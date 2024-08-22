using MediatR;

using SnowWarden.Backend.Core.Abstractions.Events;

using SnowWarden.Backend.Infrastructure.Messaging;

namespace SnowWarden.Backend.API.Jobs;

internal sealed class EventProcessorJob(
	EventChannel queue,
	IServiceScopeFactory serviceScopeFactory,
	ILogger<EventProcessorJob> logger)
	: BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await foreach (IEvent @event in queue.Reader.ReadAllAsync(stoppingToken))
		{
			try
			{
				using IServiceScope scope = serviceScopeFactory.CreateScope();
				IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

				if (DateTime.UtcNow < @event.NextOnFail)
				{
					await queue.Writer.WriteAsync(@event, stoppingToken);
					continue;
				}

				logger.LogInformation(
					"Publishing event: {EventKey} {EventType}",
					@event.EventId, @event.GetType().Name);

				await publisher.Publish(@event, stoppingToken);
			}
			catch (Exception ex)
			{
				@event.ScheduleNext();
				logger.LogError(
					ex,
					"Something went wrong! {EventId}, rewriting event, will try again in {Timeout} seconds",
					@event.EventId, @event.TimeoutSeconds);
				await queue.Writer.WriteAsync(@event, stoppingToken);
			}
		}
	}
}