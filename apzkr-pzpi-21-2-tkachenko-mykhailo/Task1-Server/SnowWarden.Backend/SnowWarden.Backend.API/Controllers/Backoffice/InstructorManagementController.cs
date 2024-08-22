using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SnowWarden.Backend.API.Extensions;
using SnowWarden.Backend.API.Extensions.Members;
using SnowWarden.Backend.API.Models.Response;
using SnowWarden.Backend.API.Models.Response.Dtos;

using SnowWarden.Backend.Core.Exceptions;
using SnowWarden.Backend.Core.Features.Identity.Services;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Features.Members.Services;
using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;
using SnowWarden.Backend.Core.Utils.Results;

using RegisterRequest = SnowWarden.Backend.API.Models.Requests.Auth.RegisterRequest;

namespace SnowWarden.Backend.API.Controllers.Backoffice;

[Route("backoffice/instructors")]
public class InstructorManagementController(
	IIdentityService<Admin> adminIdentityService,
	IInstructorService instructorService,
	IMapper mapper) : ControllerBase
{
	[Route("")]
	[HttpGet]
	[Authorize(Roles = Admin.ROLE_NAME)]
	public async Task<IActionResult> Get()
	{
		IReadOnlyCollection<Instructor> instructors = await instructorService.GetAsync();
		IEnumerable<UserResponseDto> mapped = instructors.Select(mapper.Map<Instructor, UserResponseDto>);

		return ResponseObject<IEnumerable<UserResponseDto>>
			.Status200Ok(mapped)
			.ToObjectResult();
	}

	[Route("")]
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] RegisterRequest request)
	{
		if (!ModelState.IsValid)
		{
			return FailedValidationResponse();
		}

		Instructor instructor = new()
		{
			FirstName = request.FirstName,
			LastName = request.LastName,
			Email = request.Email
		};
		instructor.SetLanguage(request.Language);

		try
		{
			ApplicationIdentityResult<Instructor> result = await instructorService.CreateInstructorAccount(instructor);

			if (!result.Succeeded)
			{
				result.WithLanguage(User.Language());

				return ResponseObject<UserResponseDto>
					.Failure(400, result.ToResponseErrors().ToArray())
					.ToObjectResult();
			}

			UserResponseDto mapped = mapper.Map<Instructor, UserResponseDto>(result.User!);
			return ResponseObject<UserResponseDto>
				.Status201Created(mapped)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<UserResponseDto>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[Route("{id:int}")]
	[HttpDelete]
	[Authorize(Roles=Admin.ROLE_NAME)]
	public async Task<IActionResult> Delete([FromRoute] int id)
	{
		ApplicationIdentityResult<Instructor> instructorResult = await instructorService.GetByIdAsync(id);
		string adminEmail = User.Identity?.Name ?? string.Empty;
		ApplicationIdentityResult<Admin> adminResult = await adminIdentityService.FindAsync(adminEmail);

		if (!instructorResult.Succeeded)
		{
			return ResponseObject<Instructor>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}

		if (!adminResult.Succeeded)
		{
			LocalizedContent localizedError = new()
			{
				Translations =
				{
					{
						Localizator.SupportedLanguages.Ukrainian,
						"Невалідний адміністратор"
					},
					{
						Localizator.SupportedLanguages.AmericanEnglish,
						"Invalid administrator"
					}
				}
			};
			return ResponseObject<string>.Failure(
				500,
				errors: new ResponseError(
					errorCode: ResponseError.ErrorCodes.Unspecified,
					message: localizedError.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}

		LocalizedContent localizedContent = new()
		{
			Translations =
			{
				{
					Localizator.SupportedLanguages.Ukrainian,
					$"Акаунт інструктора {instructorResult.User.Email} було успішно деактивовано"
				},
				{
					Localizator.SupportedLanguages.AmericanEnglish,
					$"Instructor account {instructorResult.User.Email} has been disabled successfully"
				}
			}
		};

		await instructorService.DisableAsync(instructorResult.User, adminResult.User);

		return ResponseObject<string>
			.Success(localizedContent.ToLocalizedString(User.Language()))
			.ToObjectResult();
	}

	private ObjectResult FailedValidationResponse()
	{
		IEnumerable<ResponseError> errors = ModelState.Values
			.SelectMany(v => v.Errors)
			.Select(e =>
				ResponseError.ValidationError(e.ErrorMessage));

		return ResponseObject<object>
			.Failure(errors, 400)
			.ToObjectResult();
	}
}