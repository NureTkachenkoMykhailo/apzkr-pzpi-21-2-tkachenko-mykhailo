using System.Collections.Frozen;
using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SnowWarden.Backend.API.Extensions.Members;
using SnowWarden.Backend.API.Models.Response;
using SnowWarden.Backend.API.Models.Response.Dtos;

using SnowWarden.Backend.Core.Exceptions;

using SnowWarden.Backend.Core.Features.Booking;
using SnowWarden.Backend.Core.Features.Booking.Services;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Identity.Services;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Features.Training;
using SnowWarden.Backend.Core.Features.Training.Services;

using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;
using SnowWarden.Backend.Core.Utils.Results;

namespace SnowWarden.Backend.API.Controllers;

[Route("bookings")]
[Authorize]
public class BookingController(
	IBookingService bookingService,
	ITrainingService trainingService,
	IIdentityService<Guest> identityService,
	IMapper mapper) : ControllerBase
{
	[Route("")]
	[HttpGet]
	public async Task<IActionResult> GetMyBookings()
	{
		ApplicationIdentityResult<Guest> searchResult = await identityService.FindAsync(User.Identity?.Name ?? string.Empty);

		if (searchResult.User is null)
		{
			return ResponseObject<ApplicationUser>
				.IdentityNotFound(User.Identity?.Name ?? string.Empty)
				.ToObjectResult();
		}

		IReadOnlyCollection<Booking> bookings = await bookingService.GetUserBookings(searchResult.User.Id);
		FrozenSet<BookingResponseDto> mapped = bookings.Select(mapper.Map<Booking, BookingResponseDto>).ToFrozenSet();
		if (!bookings.Any())
		{
			return ResponseObject<IReadOnlyCollection<BookingResponseDto>>
				.Status204NoContent<BookingResponseDto>()
				.ToObjectResult();
		}

		return ResponseObject<FrozenSet<BookingResponseDto>>
			.Status200Ok(mapped)
			.ToObjectResult();
	}

	[Route("{trainingId:int}")]
	[HttpPost]
	[Authorize(Roles = $"{Guest.ROLE_NAME}")]
	public async Task<IActionResult> CreateBooking([FromRoute] int trainingId)
	{
		ApplicationIdentityResult<Guest> searchResult = await identityService.FindAsync(User.Identity?.Name ?? string.Empty);
		if (searchResult.User is null)
		{
			return ResponseObject<ApplicationUser>
				.IdentityNotFound(User.Identity?.Name ?? string.Empty)
				.ToObjectResult();
		}

		TrainingSession? training = await trainingService.GetByIdAsync(trainingId);
		if (training is null)
		{
			return ResponseObject<TrainingSessionResponseDto>
				.ResourceNotFoundResult(trainingId, User.Language())
				.ToObjectResult();
		}

		Booking booking = new()
		{
			TrainingId = training.Id,
			GuestId = searchResult.User.Id,
			Guest = searchResult.User,
			TrainingSession = training
		};

		try
		{
			Booking created = await bookingService.CreateAsync(booking);
			BookingResponseDto? mapped = mapper.Map<Booking, BookingResponseDto>(created);
			return ResponseObject<BookingResponseDto>
				.Status200Ok(mapped)
				.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<BookingResponseDto>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}

	[Route("{bookingId:int}")]
	[HttpPatch]
	[Authorize(Roles = $"" +
		$"{Admin.ROLE_NAME}," +
		$"{Guest.ROLE_NAME}")]
	public async Task<IActionResult> CancelBooking([FromRoute] int bookingId)
	{
		ApplicationIdentityResult<Guest> searchResult = await identityService.FindAsync(User.Identity?.Name ?? string.Empty);
		if (searchResult.User is null)
		{
			return ResponseObject<ApplicationUser>
				.IdentityNotFound(User.Identity?.Name ?? string.Empty)
				.ToObjectResult();
		}

		Booking? booking = (await bookingService.GetUserBookings(searchResult.User.Id))
			.FirstOrDefault(b => b.GuestId == searchResult.User.Id);
		if (booking is null)
		{
			return ResponseObject<BookingResponseDto>
				.ResourceNotFoundResult(bookingId, User.Language())
				.ToObjectResult();
		}

		try
		{
			await bookingService.CancelBooking(booking);

			LocalizedContent response = new()
			{
				Translations =
				{
					{
						Localizator.SupportedLanguages.AmericanEnglish,
						$"Booking with id {bookingId} has been cancelled successfully"
					},
					{
						Localizator.SupportedLanguages.Ukrainian,
						$"Бронювання з ідентифікатором {bookingId} було скасовано успішно"
					}
				}
			};

			return ResponseObject<string>
				.Status200Ok(response.ToLocalizedString(User.Language()))
			 	.ToObjectResult();
		}
		catch (LocalizedException ex)
		{
			return ResponseObject<BookingResponseDto>
				.Failure(
					500,
					new ResponseError(
						ResponseError.ErrorCodes.ResourceError,
						ex.Message.ToLocalizedString(User.Language())))
				.ToObjectResult();
		}
	}
}