using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using SnowWarden.Backend.API.Extensions;
using SnowWarden.Backend.API.Extensions.Members;
using SnowWarden.Backend.API.Models.Requests.Auth;
using SnowWarden.Backend.API.Models.Response;
using SnowWarden.Backend.API.Options.Auth;
using SnowWarden.Backend.API.Security.Tokens.Jwt;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Identity.Services;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;
using SnowWarden.Backend.Core.Utils.Results;

namespace SnowWarden.Backend.API.Controllers.Backoffice;

[ApiController]
[Route("backoffice")]
public class BackofficeController : ControllerBase
{
	private readonly JwtOptions _jwtOptions;
	private readonly IIdentityService<Instructor> _instructorIdentityService;
	private readonly IIdentityService<Admin> _adminIdentityService;

	public BackofficeController(
		IOptions<JwtOptions> jwtOptions,
		IIdentityService<Instructor> instructorIdentityService,
		IIdentityService<Admin> adminIdentityService)
	{
		_instructorIdentityService = instructorIdentityService;
		_adminIdentityService = adminIdentityService;
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
			ApplicationIdentityResult adminSignIn = await _adminIdentityService.SignIn(
				authenticateRequest.Contact,
				authenticateRequest.Password);
			ApplicationIdentityResult loginResult = adminSignIn;

			if (adminSignIn.ResultType is IdentityResultType.InvalidContact)
			{
				ApplicationIdentityResult instructorSignIn = await _instructorIdentityService.SignIn(
					authenticateRequest.Contact,
					authenticateRequest.Password);
				loginResult = instructorSignIn;
			}

			if (!loginResult.Succeeded)
			{
				return ResponseObject<IdentityToken>
					.Failure(
						loginResult.ResolveStatusCode(),
						new ResponseError(
							loginResult.ResolveErrorCode(),
							loginResult.WithLanguage(User.Language()).GetLocalizedResult().Errors.FirstOrDefault() ?? "No concrete error message provided"))
					.ToObjectResult();
			}

			IEnumerable<Claim>? principal = loginResult.User?.Principal();

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
}