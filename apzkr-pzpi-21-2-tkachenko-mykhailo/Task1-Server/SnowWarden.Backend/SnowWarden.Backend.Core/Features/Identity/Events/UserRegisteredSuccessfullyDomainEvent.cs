using SnowWarden.Backend.Core.Abstractions.Events;

namespace SnowWarden.Backend.Core.Features.Identity.Events;

public class UserRegisteredSuccessfullyDomainEvent(ApplicationUser user, string? tempPassword = null) : DomainEvent
{
	public string? TempPassword { get; init; } = tempPassword;
	public ApplicationUser Target { get; init; } = user;
}