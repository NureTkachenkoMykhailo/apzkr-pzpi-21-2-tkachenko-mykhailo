using System.Security.Claims;
using IdentityModel;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Members;

namespace SnowWarden.Backend.API.Security.Principals;

public class AdminPrincipal(ApplicationUser appUser) : ApplicationPrincipal(appUser)
{
	public override IEnumerable<Claim> PrincipalClaims()
	{
		return base.PrincipalClaims().Concat(
		[
			new Claim(JwtClaimTypes.Role, Admin.ROLE_NAME)
		]);
	}
}