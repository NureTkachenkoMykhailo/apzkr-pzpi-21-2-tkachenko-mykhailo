using System.Security.Claims;

namespace SnowWarden.Backend.API.Security.Principals;

public interface IApplicationPrincipal
{
	public IEnumerable<Claim> PrincipalClaims();
}