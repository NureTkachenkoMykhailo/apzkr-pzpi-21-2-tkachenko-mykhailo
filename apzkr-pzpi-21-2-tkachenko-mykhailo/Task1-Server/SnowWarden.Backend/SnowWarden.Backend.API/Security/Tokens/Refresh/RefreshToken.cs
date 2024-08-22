using System.Security.Cryptography;

namespace SnowWarden.Backend.API.Security.Tokens.Refresh;

public class RefreshToken
{
	public string Value { get; private set; }
	public DateTime ExpiresAt { get; private set; }

	public static RefreshToken Generate(int expirationHours)
	{
		byte[] randomNumber = new byte[64];

		using RandomNumberGenerator rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);

		return new RefreshToken
		{
			Value = Convert.ToBase64String(randomNumber),
			ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
		};
	}
}