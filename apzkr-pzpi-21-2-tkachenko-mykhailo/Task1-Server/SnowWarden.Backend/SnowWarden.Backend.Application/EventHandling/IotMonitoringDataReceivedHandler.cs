using MediatR;

using Microsoft.Extensions.Logging;

using SnowWarden.Backend.Core.Features.Communications.IoT.Events;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Core.Features.Track.Services;

namespace SnowWarden.Backend.Application.EventHandling;

public class IotMonitoringDataReceivedHandler(
	ITrackSectionService trackSectionService,
	ILogger<IotMonitoringDataReceivedHandler> logger)
	: INotificationHandler<DangerousWeatherConditionsReceivedEvent>
{
	public async Task Handle(DangerousWeatherConditionsReceivedEvent @event, CancellationToken cancellationToken)
	{
		logger.LogInformation("Received dangerous weather conditions message: [{SectionId} {MessageType}]", @event.Data.TrackSectionId, @event.Data.MessageType);
		Section? section = await trackSectionService.GetByIdCompleteAsync(@event.Data.TrackSectionId);
		if (section is null)
		{
			logger.LogWarning("Section with id {SectionId} is not operational", @event.Data.TrackSectionId);
			return;
		}
		logger.LogInformation("Recalculation danger index considering disturbing message");
		section.RecalculateDangerIndex(@event.Data.Metadata ?? []);
		section.SaveLog(@event.Data);
		await trackSectionService.UpdateAsync(section);
	}
}