using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

using SnowWarden.Backend.API.Options.Auth;
using SnowWarden.Backend.API.Security.Tokens.Refresh;

namespace SnowWarden.Backend.API.Security.Tokens.Jwt;

public class IdentityToken
{
	public string AccessToken { get; private set; } = string.Empty;
	public DateTime ExpiresAt { get; private set; } = DateTime.UtcNow;
	public RefreshToken? RefreshToken { get; private set; }

	private IdentityToken() { }

	public static IdentityToken Generate(IEnumerable<Claim> claims, JwtOptions? options = null)
	{
		DateTime expirationDate = DateTime.UtcNow.AddMinutes(
			options?.ExpirationMinutes ?? ApiDefaults.DEFAULT_ACCESS_TOKEN_EXPIRATION_MINUTES);
		JwtSecurityToken token = new(
			claims: claims,
			notBefore: DateTime.UtcNow,
			expires: expirationDate,
			signingCredentials: new SigningCredentials(
				new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options?.SigningKey ?? string.Empty)),
				ApiDefaults.DEFAULT_JWT_SIGNATURE_ALGORITHM),
			issuer: options?.ValidIssuer ?? string.Empty);

		return new IdentityToken
		{
			AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
			ExpiresAt = expirationDate,
			RefreshToken = options?.RefreshTokenOptions is not null
				? RefreshToken.Generate(options.RefreshTokenOptions.ExpirationHours)
				: null
		};
	}
}