using System.Globalization;
using System.Net.Http.Json;

using SnowWardenMobile.Abstractions.Exceptions;
using SnowWardenMobile.Abstractions.Services;

using SnowWardenMobile.Extensions;

using SnowWardenMobile.Models;
using SnowWardenMobile.Models.Auth;

namespace SnowWardenMobile.Services;

public class AuthService(HttpClient httpClient) : IAuthService
{
	public async Task<bool> AuthenticateAsync(string contact, string password)
	{
		AuthenticateRequest request = new()
		{
			Contact = contact,
			Password = password
		};
		HttpResponseMessage response = await httpClient.PostAsJsonAsync("account/authenticate", request);
		ResponseObject<IdentityToken>? result = await response.Content.ReadFromJsonAsync<ResponseObject<IdentityToken>>();
		if (result?.IsSuccessfulResult ?? false)
		{
			await Task.WhenAll(
				SecureStorage.Default.SetAsync(
					SecureStorageKeys.AUTH_TOKEN,
					result.Payload?.AccessToken ?? throw new LoginRequestFailedException()),
				SecureStorage.Default.SetAsync(
					SecureStorageKeys.AUTH_TOKEN_EXPIRATION,
					result.Payload?.ExpiresAt.ToString(CultureInfo.InvariantCulture) ?? throw new LoginRequestFailedException()));
		}

		return result?.IsSuccessfulResult ?? throw new LoginRequestFailedException();
	}

	public async Task<bool> IsAuthenticatedAsync()
	{
		return
			await SecureStorage.Default.GetAccessTokenAsync() is not null &&
			await SecureStorage.Default.TokenValidAsync();
	}
	public class LoginRequestFailedException() : ApiCallException("Could not conduct login operation, try again later");
}