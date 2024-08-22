using MediatR;
using Microsoft.Extensions.Logging;
using SnowWarden.Backend.Core.Features.Booking.Events;

namespace SnowWarden.Backend.Application.EventHandling;

public class BookingSuccessfulEventHandler(ILogger<BookingSuccessfulEventHandler> logger) : INotificationHandler<BookingSuccesfulEvent>
{
	public Task Handle(BookingSuccesfulEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("Booking has been successfully created event received");

		return Task.CompletedTask;
	}
}