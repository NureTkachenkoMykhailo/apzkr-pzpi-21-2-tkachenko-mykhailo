using System.Security.Claims;
using IdentityModel;
using SnowWarden.Backend.Core;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Utils;

namespace SnowWarden.Backend.API.Security.Principals;

public class GuestPrincipal(Guest appUser) : ApplicationPrincipal(appUser)
{
	public override IEnumerable<Claim> PrincipalClaims()
	{
		return base.PrincipalClaims().Concat(
		[
			new Claim(SecurityDefaults.ClaimTypes.MembershipClaim, appUser.Membership.ToString()),
			new Claim(JwtClaimTypes.Role, Guest.ROLE_NAME)
		]);
	}
}