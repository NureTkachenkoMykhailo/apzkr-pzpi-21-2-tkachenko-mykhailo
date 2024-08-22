using Microsoft.AspNetCore.Mvc;
using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;

namespace SnowWarden.Backend.API.Models.Response;

internal partial class ResponseObject<TPayload>
{
	public int? StatusCode { get; private init; } = null;
	public bool IsSuccessfulResult { get; private init; } = false;

	public IEnumerable<ResponseError> Errors { get; private init; } = [];

	public TPayload? Payload { get; private init; }

	private ResponseObject() {}

	public static ResponseObject<TPayload> Success(TPayload? payload, int? statusCode = null)
	{
		return new ResponseObject<TPayload>
		{
			StatusCode = statusCode,
			IsSuccessfulResult = true,
			Payload = payload
		};
	}

	public static ResponseObject<TPayload> Failure(int? statusCode = null, params ResponseError[] errors)
	{
		return new ResponseObject<TPayload>
		{
			StatusCode = statusCode,
			IsSuccessfulResult = false,
			Errors = errors
		};
	}

	public static ResponseObject<TPayload> Failure(IEnumerable<ResponseError> errors, int? statusCode = null)
	{
		return new ResponseObject<TPayload>
		{
			StatusCode = statusCode,
			IsSuccessfulResult = false,
			Errors = errors
		};
	}

	public ObjectResult ToObjectResult()
	{
		if (StatusCode == StatusCodes.Status204NoContent)
		{
			return new ObjectResult(null)
			{
				StatusCode = StatusCode
			};
		}

		return new ObjectResult(this)
		{
			StatusCode = StatusCode
		};
	}
}