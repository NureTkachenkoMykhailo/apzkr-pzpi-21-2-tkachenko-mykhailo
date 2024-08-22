using AutoMapper;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using SnowWarden.Backend.API.Extensions.Members;
using SnowWarden.Backend.API.Models.Requests.Dtos;
using SnowWarden.Backend.API.Models.Response;
using SnowWarden.Backend.API.Models.Response.Dtos;

using SnowWarden.Backend.Core.Exceptions;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Core.Features.Track.Services;
using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;

namespace SnowWarden.Backend.API.Controllers.Backoffice;

[Authorize]
[Route("backoffice/tracks")]
public class BackofficeTrackController(ITrackService trackService, IMapper mapper) : ControllerBase
{
	[Route("")]
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> Get()
	{
		IReadOnlyCollection<Track> tracks = await trackService.GetReadonlyCompleteAsync();

		return !tracks.Any()
			? ResponseObject<IReadOnlyCollection<TrackResponseDto>>
				.Status204NoContent<Track>()
				.ToObjectResult()
			: ResponseObject<IReadOnlyCollection<TrackResponseDto>>
				.Status200Ok(tracks.Select(mapper.Map<Track, TrackResponseDto>).ToList())
				.ToObjectResult();
	}

	[Route("")]
	[HttpPost]
	[Authorize(Roles = Admin.ROLE_NAME)]
	public async Task<IActionResult> Post([FromBody] TrackPostDto request)
	{
		try
		{
			Track? mapped = mapper.Map<TrackPostDto, Track>(request);
			Track created = await trackService.CreateAsync(mapped);
			TrackResponseDto response = mapper.Map<Track, TrackResponseDto>(created);
			return ResponseObject<TrackResponseDto>
				.Status201Created(response)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<Track>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[Route("{id:int}")]
	[HttpPatch]
	[Authorize(Roles = Admin.ROLE_NAME)]
	public async Task<IActionResult> Patch(
		[FromRoute] int id,
		[FromBody] TrackPostDto request)
	{
		try
		{
			Track? track = mapper.Map<TrackPostDto, Track>(request);
			track.SetExistingId(id);
			Track updateResult = await trackService.UpdateAsync(track);
			TrackResponseDto mapped = mapper.Map<Track, TrackResponseDto>(updateResult);
			return ResponseObject<TrackResponseDto>
				.Status200Ok(mapped)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<Track>
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
	[Authorize(Roles = Admin.ROLE_NAME)]
	public async Task<IActionResult> Remove([FromRoute] int id)
	{
		Track? toDelete = await trackService.GetReadonlyByIdCompleteAsync(id);

		if (toDelete is null)
		{
			return TrackNotFoundResult(id);
		}

		try
		{
			Track deleted = await trackService.DeleteAsync(toDelete);
			TrackResponseDto mapped = mapper.Map<Track, TrackResponseDto>(deleted);
			return ResponseObject<TrackResponseDto>
				.Status200Ok(mapped)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<Track>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[Route("{id:int}")]
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetById(int id)
	{
		Track? toFind = await trackService.GetReadonlyByIdCompleteAsync(id);

		if (toFind is null)
		{
			return TrackNotFoundResult(id);
		}

		TrackResponseDto response = mapper.Map<Track, TrackResponseDto>(toFind);

		return ResponseObject<TrackResponseDto>.Status200Ok(response).ToObjectResult();
	}

	private ObjectResult TrackNotFoundResult(int id)
	{
		LocalizedContent errorMessage = new()
		{
			Translations =
			{
				{ Localizator.SupportedLanguages.AmericanEnglish, $"Track with id {id} was not found" },
				{ Localizator.SupportedLanguages.Ukrainian, $"Трасу з ідентифікатором {id} не було знайдено" }
			}
		};
		return ResponseObject<Track>
			.Failure(
				404,
				new ResponseError(
					ResponseError.ErrorCodes.ResourceError,
					errorMessage.ToLocalizedString(User.Language())))
			.ToObjectResult();
	}
}