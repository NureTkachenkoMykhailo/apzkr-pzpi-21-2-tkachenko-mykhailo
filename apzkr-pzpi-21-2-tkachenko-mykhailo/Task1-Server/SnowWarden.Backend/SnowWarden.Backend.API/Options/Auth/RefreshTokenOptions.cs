namespace SnowWarden.Backend.API.Options.Auth;

public class RefreshTokenOptions
{
	public int ExpirationHours { get; set; } = ApiDefaults.DEFAULT_REFRESH_TOKEN_EXPIRATION_HOURS;
}