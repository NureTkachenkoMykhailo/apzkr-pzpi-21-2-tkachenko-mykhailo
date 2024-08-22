using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;

namespace SnowWarden.Backend.API.Models.Response;

internal partial class ResponseObject<TPayload>
{
	internal static ResponseObject<TPayload> Status200Ok(TPayload? payload) => Success(payload, statusCode: 200);

	public static ResponseObject<TPayload> ResourceNotFoundResult(int id, Language language)
	{
		LocalizedContent errorMessage = new()
		{
			Translations =
			{
				{ Localizator.SupportedLanguages.AmericanEnglish, $"{typeof(TPayload).Name} with id {id} was not found" },
				{ Localizator.SupportedLanguages.Ukrainian, $"{typeof(TPayload).Name} з ідентифікатором {id} не було знайдено" }
			}
		};
		return Failure(
			404,
			new ResponseError(
				ResponseError.ErrorCodes.NotFoundError,
				errorMessage.ToLocalizedString(language)));
	}

	public static ResponseObject<TPayload> IdentityNotFound(string username)
	{
		return Failure(
			404,
			new ResponseError(
				ResponseError.ErrorCodes.NotFoundError, $"Invalid identity {username}"));
	}

	public static ResponseObject<IEnumerable<TInner>> Status204NoContent<TInner>()
	{
		return ResponseObject<IEnumerable<TInner>>.Success(Enumerable.Empty<TInner>(), statusCode: 204);
	}

	public static ResponseObject<TPayload> Status201Created(TPayload resource)
	{
		return Success(resource, statusCode: 201);
	}
}