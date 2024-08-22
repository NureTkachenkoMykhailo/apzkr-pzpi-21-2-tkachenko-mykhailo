using MediatR;
using Microsoft.Extensions.Logging;
using SnowWarden.Backend.Core.Features.Booking.Events;

namespace SnowWarden.Backend.Application.EventHandling;

public class BookingCancelledEventHandler(ILogger<BookingCancelledEventHandler> logger) : INotificationHandler<BookingCancelledEvent>
{
	public Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
	{
		logger.LogInformation("Booking cancellation event has been received");

		return Task.CompletedTask;
	}
}