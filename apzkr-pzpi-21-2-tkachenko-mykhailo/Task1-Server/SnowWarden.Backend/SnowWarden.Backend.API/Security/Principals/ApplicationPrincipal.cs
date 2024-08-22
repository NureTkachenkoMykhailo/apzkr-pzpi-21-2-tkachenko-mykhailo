using System.Security.Claims;
using IdentityModel;
using SnowWarden.Backend.Core;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Utils;

namespace SnowWarden.Backend.API.Security.Principals;

public abstract class ApplicationPrincipal(ApplicationUser appUser) : IApplicationPrincipal
{
	public virtual IEnumerable<Claim> PrincipalClaims()
	{
		yield return new Claim(JwtClaimTypes.Email, appUser.Email ?? string.Empty);
		yield return new Claim(JwtClaimTypes.PreferredUserName, appUser.UserName ?? string.Empty);
		yield return new Claim(SecurityDefaults.ClaimTypes.Language, appUser.Language.Code);
	}
}