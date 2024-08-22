using System.Collections.Generic;

namespace SnowWardenMobile.Models;

public class ResponseObject<TPayload>
{
	public int? StatusCode { get; init; } = null;
	public bool IsSuccessfulResult { get; init; } = false;

	public IEnumerable<ResponseError> Errors { get; init; } = [];

	public TPayload? Payload { get; init; }
}

public record ResponseError
{
	public static class ErrorCodes
	{
		public const string UnknownError = "UE";
		public const string ValidationError = "VE";
		public const string InvalidCredentials = "ICE";
		public const string NotFoundError = "NFE";
		public const string LockdownError = "LDE";
		public const string ResourceError = "RE";
		public const string LocalizationError = "LE";
		public const string EmailNotConfirmed = "ENC";
		public const string Unspecified = "USPE";
	}

	public string ErrorCode { get; init; }
	public string Message { get; init; }
}