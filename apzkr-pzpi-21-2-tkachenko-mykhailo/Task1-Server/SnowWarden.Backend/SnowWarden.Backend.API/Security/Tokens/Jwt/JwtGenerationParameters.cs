using SnowWarden.Backend.API.Security.Tokens.Refresh;

namespace SnowWarden.Backend.API.Security.Tokens.Jwt;

public class JwtGenerationParameters
{
	public string Issuer { get; set; } = string.Empty;
	public string SigningKey { get; set; } = string.Empty;
	public int ExpirationMinutes { get; set; } = ApiDefaults.DEFAULT_ACCESS_TOKEN_EXPIRATION_MINUTES;

	public RefreshTokenParameters? RefreshTokenParameters { get; set; }
}