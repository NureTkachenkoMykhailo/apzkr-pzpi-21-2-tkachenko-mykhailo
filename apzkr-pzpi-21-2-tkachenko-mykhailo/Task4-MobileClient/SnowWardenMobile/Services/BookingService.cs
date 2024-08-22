using System.Net;
using System.Net.Http.Json;
using SnowWardenMobile.Abstractions.Exceptions;
using SnowWardenMobile.Abstractions.Services;
using SnowWardenMobile.Extensions;
using SnowWardenMobile.Models;

namespace SnowWardenMobile.Services;

public class BookingService(HttpClient client, IAuthService authService) : IBookingService
{
	/// <inheritdoc />
	public async Task<Booking> Create(int trainingId)
	{
		await CheckExcess();
		HttpResponseMessage response  = await (await client.WithAuthorization()).PostAsync($"{trainingId}", null);
		ResponseObject<Booking>? responseObject  = await response.Content.ReadFromJsonAsync<ResponseObject<Booking>>();

		return
			(responseObject?.IsSuccessfulResult ?? false)
				? responseObject.Payload ?? throw new BookingRequestFailedException()
				: throw new BookingRequestFailedException();
	}

	/// <inheritdoc />
	public async Task<ICollection<Booking>> GetBookings()
	{
		await CheckExcess();
		HttpResponseMessage response  = await (await client.WithAuthorization()).GetAsync("");
		ResponseObject<ICollection<Booking>>? responseObject = null;
		if (response.StatusCode == HttpStatusCode.NoContent)
		{
			responseObject = new ResponseObject<ICollection<Booking>>()
			{
				IsSuccessfulResult = true,
				Payload = [],
				StatusCode = 204
			};
		}
		else
		{
			responseObject = await response.Content.ReadFromJsonAsync<ResponseObject<ICollection<Booking>>>();
		}

		return
			(responseObject?.IsSuccessfulResult ?? false)
				? responseObject.Payload ?? throw new BookingRequestFailedException()
				: throw new BookingRequestFailedException();
	}

	private async Task CheckExcess()
	{
		if (await authService.IsAuthenticatedAsync() is false)
		{
			throw new BookingAuthorizationException();
		}
	}

	public class BookingAuthorizationException() : ApiCallException("Client is not authorized to perform booking action");

	public class BookingRequestFailedException() : ApiCallException("Could not retrieve booking results");
}