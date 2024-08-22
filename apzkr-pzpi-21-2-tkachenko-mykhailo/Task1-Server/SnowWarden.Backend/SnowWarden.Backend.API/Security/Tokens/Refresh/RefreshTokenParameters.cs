namespace SnowWarden.Backend.API.Security.Tokens.Refresh;

public class RefreshTokenParameters
{
	public int ExpirationHours { get; set; } = ApiDefaults.DEFAULT_REFRESH_TOKEN_EXPIRATION_HOURS;
}