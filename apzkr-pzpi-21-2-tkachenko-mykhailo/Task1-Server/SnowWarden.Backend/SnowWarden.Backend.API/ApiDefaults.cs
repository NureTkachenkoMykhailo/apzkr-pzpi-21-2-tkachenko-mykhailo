using Microsoft.IdentityModel.Tokens;

namespace SnowWarden.Backend.API;

public static class ApiDefaults
{
	public const int DEFAULT_REFRESH_TOKEN_EXPIRATION_HOURS = 1;
	public const int DEFAULT_ACCESS_TOKEN_EXPIRATION_MINUTES = 1;
	public const string DEFAULT_JWT_SIGNATURE_ALGORITHM = SecurityAlgorithms.HmacSha256;
}