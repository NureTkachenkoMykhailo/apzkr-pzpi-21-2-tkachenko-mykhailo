namespace SnowWarden.Backend.API.Options.Auth;

public class JwtOptions
{
	public string ValidIssuer { get; set; }
	public string SigningKey { get; set; }
	public int ExpirationMinutes { get; set; }
	public RefreshTokenOptions RefreshTokenOptions { get; set; }
}