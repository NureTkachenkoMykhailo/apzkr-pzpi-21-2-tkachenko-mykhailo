using System.ComponentModel.DataAnnotations;

namespace SnowWarden.Backend.API.Models.Requests.Auth;

public record Credentials(
	[Required] [EmailAddress] string Email,
	[Required] string Password);