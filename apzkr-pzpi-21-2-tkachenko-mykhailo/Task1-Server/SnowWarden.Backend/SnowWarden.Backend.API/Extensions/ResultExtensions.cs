using SnowWarden.Backend.API.Models.Response;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Utils.Results;

namespace SnowWarden.Backend.API.Extensions;

public static class ResultExtensions
{
	public static int ResolveStatusCode(this ApplicationIdentityResult result)
	{
		return result.ResultType switch
		{
			IdentityResultType.InvalidContact or IdentityResultType.InvalidPassword => 404,
			IdentityResultType.LockedOut or IdentityResultType.EmailNotConfirmed or IdentityResultType.Invalid => 403,
			_ => 500
		};

	}

	public static string ResolveErrorCode(this ApplicationIdentityResult result)
	{
		return result.ResultType switch
		{
			IdentityResultType.InvalidContact or IdentityResultType.InvalidPassword => ResponseError.ErrorCodes.InvalidCredentials,
			IdentityResultType.LockedOut => ResponseError.ErrorCodes.LockdownError,
			IdentityResultType.EmailNotConfirmed => ResponseError.ErrorCodes.EmailNotConfirmed,
			IdentityResultType.Invalid => ResponseError.ErrorCodes.Unspecified,
		};
	}

	public static IEnumerable<ResponseError> ToResponseErrors(this ApplicationIdentityResult result)
	{
		string errorCode = result.ResolveErrorCode();
		return result.GetLocalizedResult().Errors.Select(e => new ResponseError(errorCode, e)).ToArray();
	}
}