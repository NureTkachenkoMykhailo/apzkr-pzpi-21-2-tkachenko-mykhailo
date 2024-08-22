using System.ComponentModel.DataAnnotations;

using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.API.Models.Requests.Auth;

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

	public required string Language { get; init; } = Localizator.SupportedLanguages.AmericanEnglish.Code;

	public bool Enable2fa { get; init; } = false;
}