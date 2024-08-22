using System.Security.Claims;
using IdentityModel;
using SnowWarden.Backend.Core;
using SnowWarden.Backend.Core.Features.Members;

namespace SnowWarden.Backend.API.Security.Principals;

public class InstructorPrincipal(Instructor instructor) : ApplicationPrincipal(instructor)
{
	public override IEnumerable<Claim> PrincipalClaims()
	{
		return base.PrincipalClaims().Concat(
		[
			new Claim(JwtClaimTypes.Role, Instructor.ROLE_NAME)
		]);
	}
}