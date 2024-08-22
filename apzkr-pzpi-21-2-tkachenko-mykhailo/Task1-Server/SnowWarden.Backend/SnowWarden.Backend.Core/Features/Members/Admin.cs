using SnowWarden.Backend.Core.Features.Identity;

namespace SnowWarden.Backend.Core.Features.Members;

public class Admin : ApplicationUser
{
	public const string ROLE_NAME = "Admin";
}