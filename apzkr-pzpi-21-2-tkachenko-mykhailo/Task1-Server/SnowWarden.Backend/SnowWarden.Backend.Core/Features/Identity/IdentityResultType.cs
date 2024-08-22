namespace SnowWarden.Backend.Core.Features.Identity;

public enum IdentityResultType
{
	Valid,
	InvalidContact,
	InvalidPassword,
	LockedOut,
	Invalid,
	EmailNotConfirmed
}