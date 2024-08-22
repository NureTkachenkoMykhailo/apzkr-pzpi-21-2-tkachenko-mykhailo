using System.ComponentModel.DataAnnotations;

namespace SnowWardenMobile.Models.Auth;

public class RegisterRequest
{
	[Required]
	public required string FirstName { get; init; }

	[Required]
	public required string LastName { get; init; }

	[Required]
	[EmailAddress]
	public required string Email { get; init; }

	[Required]
	public required string Password { get; init; }

	public required string Language { get; init; } = "en-US";
}