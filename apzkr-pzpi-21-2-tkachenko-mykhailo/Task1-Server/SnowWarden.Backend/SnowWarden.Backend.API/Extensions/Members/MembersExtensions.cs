using System.Security.Claims;
using IdentityModel;

using SnowWarden.Backend.API.Security.Principals;

using SnowWarden.Backend.Core;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Utils;
using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.API.Extensions.Members;

public static class MembersExtensions
{
	public static IEnumerable<Claim> Principal<TMember>(this TMember member) where TMember : ApplicationUser
	{
		return member switch
		{
			Guest guest => new GuestPrincipal(guest).PrincipalClaims(),
			Instructor instructor => new InstructorPrincipal(instructor).PrincipalClaims(),
			Admin admin => new AdminPrincipal(admin).PrincipalClaims(),
			_ => []
		};
	}

	public static string? Email(this ClaimsPrincipal? user) =>
		user?.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?.Value;

	public static Language Language(this ClaimsPrincipal? user)
	{
		string languageCode = user?.Claims.FirstOrDefault(c =>
			c.Type == SecurityDefaults.ClaimTypes.Language)?.Value
			?? Localizator.SupportedLanguages.AmericanEnglish.Code;
		Language language = Localizator.SupportedLanguages.LanguageResolver.GetLanguage(languageCode);
		return language;
	}
}