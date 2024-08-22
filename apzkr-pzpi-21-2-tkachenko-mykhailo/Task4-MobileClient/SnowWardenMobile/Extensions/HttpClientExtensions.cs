namespace SnowWardenMobile.Extensions;

public static class HttpClientExtensions
{
	public static async Task<HttpClient> WithAuthorization(this HttpClient client)
	{
		if (client.DefaultRequestHeaders.Any(drh => drh.Key == "Authorization"))
			return client;

		string? token = await SecureStorage.Default.GetAccessTokenAsync();
		client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

		return client;
	}
}