using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Options;

using SnowWarden.Backend.API.Extensions;
using SnowWarden.Backend.API.Extensions.Members;
using SnowWarden.Backend.API.Models.Requests.Account;
using SnowWarden.Backend.API.Models.Requests.Auth;
using SnowWarden.Backend.API.Models.Response;
using SnowWarden.Backend.API.Options.Auth;
using SnowWarden.Backend.API.Security.Tokens.Jwt;

using SnowWarden.Backend.Core.Exceptions;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Identity.Services;
using SnowWarden.Backend.Core.Features.Members;

using SnowWarden.Backend.Core.Utils.Localization.Extensions;
using SnowWarden.Backend.Core.Utils.Results;

using RegisterRequest = SnowWarden.Backend.API.Models.Requests.Auth.RegisterRequest;

namespace SnowWarden.Backend.API.Controllers;

[ApiController]
[Route("account")]
public class GuestAccountController : ControllerBase
{
	private readonly JwtOptions _jwtOptions;
	private readonly IIdentityService<Guest> _guestIdentityService;
	private readonly IIdentityService<ApplicationUser> _identityService;

	public GuestAccountController(
		IOptions<JwtOptions> jwtOptions,
		IIdentityService<Guest> guestIdentityService, IIdentityService<ApplicationUser> identityService)
	{
		_guestIdentityService = guestIdentityService;
		_identityService = identityService;
		_jwtOptions = jwtOptions.Value;
	}

	[HttpPost]
	[Route("authenticate")]
	public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest authenticateRequest)
	{
		if (!ModelState.IsValid)
		{
			return FailedValidationResponse();
		}
		try
		{
			ApplicationIdentityResult<Guest> loginIdentityResult = await _guestIdentityService.SignIn(
				authenticateRequest.Contact,
				authenticateRequest.Password);

			if (!loginIdentityResult.Succeeded)
			{
				return ResponseObject<IdentityToken>
					.Failure(
						loginIdentityResult.ResolveStatusCode(),
						new ResponseError(
							loginIdentityResult.ResolveErrorCode(),
							loginIdentityResult.WithLanguage(User.Language()).GetLocalizedResult().Errors.FirstOrDefault() ?? "No concrete error message provided"))
					.ToObjectResult();
			}

			IEnumerable<Claim>? principal = loginIdentityResult.User?.Principal();
			IdentityToken token = IdentityToken.Generate(
				principal ?? throw new InvalidOperationException("Login operation has succeeded, but specified claims could not be retrieved, try again later"),
				_jwtOptions);

			return ResponseObject<IdentityToken>
				.Status200Ok(token)
				.ToObjectResult();
		}
		catch (Exception ex)
		{
			return ResponseObject<IdentityToken>
				.Failure(500, ResponseError.UnknownError(ex.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[HttpPost]
	[Route("register")]
	public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
	{
		if (!ModelState.IsValid)
		{
			return FailedValidationResponse();
		}

		try
		{
			Guest guest = new()
			{
				FirstName = registerRequest.FirstName,
				LastName = registerRequest.LastName,
				Email = registerRequest.Email
			};
			guest.SetLanguage(registerRequest.Language);
			ApplicationIdentityResult<Guest> registerIdentityResult = await _guestIdentityService.Register(guest, registerRequest.Password);

			if (!registerIdentityResult.Succeeded)
			{
				return FailedIdentityResponse(registerIdentityResult);
			}

			Guest registered = registerIdentityResult.User ?? throw new InvalidOperationException("Something went wrong");

			IdentityToken token = IdentityToken.Generate(registered.Principal(), _jwtOptions);

			return ResponseObject<IdentityToken>
				.Status200Ok(token)
				.ToObjectResult();
		}
		catch (Exception ex)
		{
			return ResponseObject<IdentityToken>
				.Failure(500, ResponseError.UnknownError(ex.Message))
				.ToObjectResult();
		}
	}

	[HttpPost]
	[Route("confirm-email")]
	[Authorize]
	public async Task<IActionResult> ConfirmEmail(
		[FromQuery] int userId,
		[FromQuery] string token)
	{
		ApplicationIdentityResult<Guest> confirmEmailIdentityResult = await _guestIdentityService.ConfirmEmail(userId, token);

		switch (confirmEmailIdentityResult.Succeeded)
		{
			case true:
				return ResponseObject<object>
					.Status200Ok(null)
					.ToObjectResult();
			case false:
				confirmEmailIdentityResult.WithLanguage(User.Language());
				int statusCode = confirmEmailIdentityResult.ResolveStatusCode();
				ResponseError[] errors = confirmEmailIdentityResult.ToResponseErrors().ToArray();

				return ResponseObject<object>
					.Failure(statusCode ,errors: errors)
					.ToObjectResult();
		}
	}

	[HttpPatch]
	[Route("")]
	[Authorize(Roles =
		$"{Admin.ROLE_NAME}," +
		$"{Instructor.ROLE_NAME}," +
		$"{Guest.ROLE_NAME}")]
	public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
	{
		try
		{
			ApplicationIdentityResult<ApplicationUser> searchIdentityResult =
				await _identityService.FindAsync(User.Identity?.Name ?? User.Email() ?? string.Empty);

			if (searchIdentityResult.Succeeded)
			{
				request.CopyTo(searchIdentityResult.User!);
				ApplicationIdentityResult<ApplicationUser> updateIdentityResult =
					await _identityService.UpdateAsync(searchIdentityResult.User!);

				return ResponseObject<ApplicationUser>
					.Status200Ok(updateIdentityResult.User!)
					.ToObjectResult();
			}

			IApplicationResult localizedResult =
				searchIdentityResult.WithLanguage(User.Language()).GetLocalizedResult();
			string errorCode = searchIdentityResult.ResolveErrorCode();
			ResponseError[] searchResultErrors =
				localizedResult.Errors.Select(e => new ResponseError(errorCode, e)).ToArray();

			return ResponseObject<Guest>
				.Failure(
					statusCode: searchIdentityResult.ResolveStatusCode(),
					errors: searchResultErrors)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<ApplicationUser>
				.Failure(500, new ResponseError(
					ResponseError.ErrorCodes.LocalizationError,
					ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
		catch (InvalidLocalizationException ex)
		{
			return ResponseObject<ApplicationUser>
				.Failure(400, new ResponseError(
					ResponseError.ErrorCodes.LocalizationError,
					ex.LocalizedMessage.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[HttpGet]
	[Route("")]
	[Authorize(Roles =
		$"{Admin.ROLE_NAME}," +
		$"{Instructor.ROLE_NAME}," +
		$"{Guest.ROLE_NAME}")]
	public IActionResult Get()
	{
		return ResponseObject<object>
			.Status200Ok(new
			{
				User.Identity?.Name,
				Claims = User.Claims.Select(c => new { c.Type, c.Value })
			})
			.ToObjectResult();
	}

	private ObjectResult FailedValidationResponse()
	{
		IEnumerable<ResponseError> errors = ModelState.Values
			.SelectMany(v => v.Errors)
			.Select(e =>
				ResponseError.ValidationError(e.ErrorMessage));

		return ResponseObject<IdentityToken>
			.Failure(errors, 400)
			.ToObjectResult();
	}

	private static ObjectResult FailedIdentityResponse(ApplicationIdentityResult applicationResult)
	{
		int statusCode = applicationResult.ResolveStatusCode();
		ResponseError[] errors = applicationResult.ToResponseErrors().ToArray();

		return ResponseObject<IdentityToken>
			.Failure(statusCode, errors)
			.ToObjectResult();
	}
}