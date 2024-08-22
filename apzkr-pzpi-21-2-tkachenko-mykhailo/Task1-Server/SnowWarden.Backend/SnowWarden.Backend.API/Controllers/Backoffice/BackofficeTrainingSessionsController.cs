using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SnowWarden.Backend.API.Extensions.Members;
using SnowWarden.Backend.API.Models.Requests.Dtos;
using SnowWarden.Backend.API.Models.Response;
using SnowWarden.Backend.API.Models.Response.Dtos;

using SnowWarden.Backend.Core.Exceptions;
using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Inventory.Services;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Features.Training;
using SnowWarden.Backend.Core.Features.Training.Services;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;

namespace SnowWarden.Backend.API.Controllers.Backoffice;

[Route("backoffice/trainings")]
[Authorize(Roles = $"" +
	$"{Admin.ROLE_NAME}," +
	$"{Instructor.ROLE_NAME}")]
public class BackofficeTrainingSessionsController(
	ITrainingService trainingService,
	IInventoryItemService inventoryItemService,
	IMapper mapper) : ControllerBase
{
	[HttpGet]
	[Route("")]
	[AllowAnonymous]
	public async Task<IActionResult> Get()
	{
		try
		{
			List<TrainingSession> trainings = await trainingService.GetCompleteAsync();
			List<TrainingSessionResponseDto> mapped = trainings
				.Select(mapper.Map<TrainingSession, TrainingSessionResponseDto>)
				.ToList();

			if (!mapped.Any())
			{
				 return ResponseObject<IEnumerable<TrainingSessionResponseDto>>
					.Status204NoContent<TrainingSessionResponseDto>()
					.ToObjectResult();
			}

			return ResponseObject<IEnumerable<TrainingSessionResponseDto>>
				.Status200Ok(mapped)
				.ToObjectResult();
		}
		catch (Exception ex)
		{
			if (ex is LocalizedException)
				return ResponseObject<object>
					.Failure(
						400,
						[
							new ResponseError(
								ResponseError.ErrorCodes.UnknownError,
								ex.ToLocalizedString(User.Language()))
						])
					.ToObjectResult();

			return ResponseObject<object>
				.Failure(400, new ResponseError(ResponseError.ErrorCodes.UnknownError, ex.Message))
				.ToObjectResult();
		}
	}

	[Route("")]
	[HttpPost]
	public async Task<IActionResult> Post([FromBody] TrainingSessionPostDto trackDto)
	{
		try
		{
			TrainingSession? training = mapper.Map<TrainingSessionPostDto, TrainingSession>(trackDto);
			TrainingSession created = await trainingService.CreateAsync(training);
			TrainingSession? read = await trainingService.GetReadonlyByIdCompleteAsync(created.Id);
			TrainingSessionResponseDto mapped = mapper.Map<TrainingSession, TrainingSessionResponseDto>(read);
			return ResponseObject<TrainingSessionResponseDto>
				.Status201Created(mapped)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<TrainingSessionResponseDto>
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
	public async Task<IActionResult> Patch(
		[FromRoute] int id,
		[FromBody] TrainingSessionPostDto trainingDto)
	{
		try
		{
			TrainingSession? training = mapper.Map<TrainingSessionPostDto, TrainingSession>(trainingDto);
			training.SetExistingId(id);
			TrainingSession updateResult = await trainingService.UpdateAsync(training);
			TrainingSessionResponseDto mapped = mapper.Map<TrainingSession, TrainingSessionResponseDto>(updateResult);
			return ResponseObject<TrainingSessionResponseDto>
				.Status200Ok(mapped)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<TrainingSessionResponseDto>
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
	public async Task<IActionResult> Remove([FromRoute] int id)
	{
		TrainingSession? toDelete = await trainingService.GetByIdCompleteAsync(id);

		if (toDelete is null)
		{
			return ResponseObject<TrainingSession>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}

		try
		{
			TrainingSession deleted = await trainingService.DeleteAsync(toDelete);
			TrainingSessionResponseDto? mapped = mapper.Map<TrainingSession, TrainingSessionResponseDto>(deleted);
			return ResponseObject<TrainingSessionResponseDto>
				.Status200Ok(mapped)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<TrainingSessionResponseDto>
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
	public async Task<IActionResult> GetById(int id)
	{
		TrainingSession? toFind = await trainingService.GetReadonlyByIdCompleteAsync(id);

		if (toFind is null)
		{
			return ResponseObject<TrainingSession>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}

		TrainingSessionResponseDto response = mapper.Map<TrainingSession, TrainingSessionResponseDto>(toFind);

		return ResponseObject<TrainingSessionResponseDto>
			.Status200Ok(response)
			.ToObjectResult();
	}

	[Route("{id:int}/reserve")]
	[HttpPut]
	[Authorize(Roles = Instructor.ROLE_NAME)]
	public async Task<IActionResult> ReserveInventoryItems([FromRoute] int id, [FromQuery] params int[] itemIds)
	{
		TrainingSession? session = await trainingService.GetReadonlyByIdCompleteAsync(id);

		if (session is null)
		{
			return ResponseObject<TrainingSession>
				.ResourceNotFoundResult(id, User.Language())
				.ToObjectResult();
		}

		List<int> notAvailableItems = [];
		foreach (int itemId in itemIds)
		{
			InventoryItem? item = await inventoryItemService.GetReadonlyByIdCompleteAsync(itemId);

			if (item is null)
			{
				return ResponseObject<InventoryItem>
					.ResourceNotFoundResult(itemId, User.Language())
					.ToObjectResult();
			}

			TrainingSession? reservedBySession = await inventoryItemService.IsReservedForRequestedTime(
				itemId,
				session.GeneralInformation.Start,
				session.GeneralInformation.DurationMinutes);
			if (reservedBySession is not null)
			{
				notAvailableItems.Add(itemId);
			}
			else
			{
				session.InventoryItemsTook ??= new List<InventoryItem>();
				session.InventoryItemsTook.Add(item);
			}
		}

		await trainingService.UpdateAsync(session);
		TrainingSession read = await trainingService.GetReadonlyByIdCompleteAsync(session.Id)!;
		TrainingSessionResponseDto? mapped = mapper.Map<TrainingSession, TrainingSessionResponseDto>(read);
		return ResponseObject<TrainingSessionResponseDto>
			.Status200Ok(mapped)
			.ToObjectResult();
	}
}