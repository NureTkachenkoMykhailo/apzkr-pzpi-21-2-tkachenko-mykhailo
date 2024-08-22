using MediatR;

using Microsoft.Extensions.Logging;

using SnowWarden.Backend.Core.Features.Identity.Events;

namespace SnowWarden.Backend.Application.EventHandling;

public class UserConfirmedEmailEventHandler(ILogger<UserConfirmedEmailEventHandler> logger) : INotificationHandler<UserEmailConfirmedEvent>
{
	public Task Handle(UserEmailConfirmedEvent @event, CancellationToken cancellationToken)
	{
		logger.LogInformation("User {Email} has confirmed email",  @event.Target.Email);

		return Task.CompletedTask;
	}
}