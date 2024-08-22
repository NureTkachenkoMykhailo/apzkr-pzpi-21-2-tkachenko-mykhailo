using System.ComponentModel.DataAnnotations;

namespace SnowWarden.Backend.API.Models.Requests.Auth;

public class AuthenticateRequest
{
	[Required]
	public required string Contact { get; init; }

	[Required]
	public required string Password { get; init; }
}