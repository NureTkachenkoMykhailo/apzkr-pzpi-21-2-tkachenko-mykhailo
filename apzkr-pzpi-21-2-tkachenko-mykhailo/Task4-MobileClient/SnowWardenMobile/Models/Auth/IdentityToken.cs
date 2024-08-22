using System;

namespace SnowWardenMobile.Models.Auth;

public class IdentityToken
{
	public string AccessToken { get; set; } = string.Empty;
	public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
	public RefreshToken? RefreshToken { get; set; }
}

public class RefreshToken
{
	public string Value { get; set; }
	public DateTime ExpiresAt { get; set; }
}