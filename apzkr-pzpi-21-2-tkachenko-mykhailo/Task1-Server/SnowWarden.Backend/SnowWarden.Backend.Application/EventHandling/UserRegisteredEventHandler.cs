using MediatR;

using Microsoft.Extensions.Logging;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Identity.Events;
using SnowWarden.Backend.Core.Features.Identity.Services;

namespace SnowWarden.Backend.Application.EventHandling;

public class UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger, IApplicationUserManager<ApplicationUser> userManager)
	: INotificationHandler<UserRegisteredSuccessfullyDomainEvent>
{
	public async Task Handle(UserRegisteredSuccessfullyDomainEvent @event, CancellationToken cancellationToken)
	{
		string url = await userManager.GenerateEmailConfirmationTokenUrlQuery(@event.Target);
		logger.LogInformation(
			"Sending registered user {UserEmail} an email with confirmation link {ConfirmationUrl}{Extra}",
			@event.Target.Email,
			url,
			@event.TempPassword is not null ? $" and temporary password {@event.TempPassword}" : "");
	}
}