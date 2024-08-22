using SnowWarden.Backend.Core.Abstractions.Events;

namespace SnowWarden.Backend.Core.Features.Identity.Events;

public class UserEmailConfirmedEvent(ApplicationUser user) : DomainEvent
{
	public ApplicationUser Target { get; init; } = user;
}