namespace SnowWardenMobile.Extensions;

public static class SecureStorageExtensions
{
	public static async Task<string?> GetAccessTokenAsync(this ISecureStorage secureStorage)
	{
		string? token = await secureStorage.GetAsync(SecureStorageKeys.AUTH_TOKEN);

		return token;
	}

	public static async Task<bool> TokenValidAsync(this ISecureStorage secureStorage)
	{
		string dtString = await secureStorage.GetAsync(SecureStorageKeys.AUTH_TOKEN_EXPIRATION) ?? string.Empty;
		bool dtValid = DateTime.TryParse(dtString, out DateTime expireAt);
		bool hasNotExpiredYet = DateTime.UtcNow < expireAt;

		return
			dtValid &&
			hasNotExpiredYet;
	}
}