using System.Net.Http.Json;

using SnowWardenMobile.Abstractions.Exceptions;
using SnowWardenMobile.Abstractions.Services;
using SnowWardenMobile.Models;
using SnowWardenMobile.Models.Trainings;

namespace SnowWardenMobile.Services;

public class TrainingSessionService(HttpClient client) : ITrainingSessionService
{
	public async Task<ICollection<TrainingSession>> GetTrainingSessionsAsync()
	{
		HttpResponseMessage response = await client.GetAsync("backoffice/trainings");
		// var debug = await response.Content.ReadFromJsonAsync<dynamic>();
		ResponseObject<ICollection<TrainingSession>>? responseObject = await response.Content.ReadFromJsonAsync<ResponseObject<ICollection<TrainingSession>>>();
		if ((responseObject?.IsSuccessfulResult ?? false) is false)
		{
			throw new GetTrainingSessionsRequestFailedException();
		}
		ICollection<TrainingSession>? trainings = responseObject.Payload;
		return trainings ?? throw new GetTrainingSessionsRequestFailedException();
	}

	public class GetTrainingSessionsRequestFailedException() : ApiCallException("Could not fetch training sessions");
}