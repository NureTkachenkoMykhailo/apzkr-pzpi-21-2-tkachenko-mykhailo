using SnowWarden.Backend.Core.Abstractions.Events;

namespace SnowWarden.Backend.Core.Features.Members.Events;

public class InstructorAccountDisabledEvent(string adminEmail) : DomainEvent
{
	public string WhoDisabled { get; init; } = adminEmail;
}