namespace SnowWarden.Backend.API.Models.Response;

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

	public ResponseError(string errorCode, string message)
	{
		ErrorCode = errorCode;
		Message = message;
	}

	public static ResponseError UnknownError(string errorMessage) => new(ErrorCodes.UnknownError, errorMessage);
	public static ResponseError ValidationError(string errorMessage) => new(ErrorCodes.ValidationError, errorMessage);
	public static ResponseError InvalidCredentials(string errorMessage) => new(ErrorCodes.InvalidCredentials, errorMessage);

	public static ResponseError NotFound(string errorMessage) => new(ErrorCodes.NotFoundError, errorMessage);

	public static ResponseError Cooldown(string errorMessage) => new(ErrorCodes.LockdownError, errorMessage);
	public static ResponseError Plain(string errorMessage) => new("", errorMessage);
}