using System.ComponentModel.DataAnnotations;

namespace SnowWardenMobile.Models.Auth;

public class AuthenticateRequest
{
	[Required]
	public required string Contact { get; init; }

	[Required]
	public required string Password { get; init; }
}