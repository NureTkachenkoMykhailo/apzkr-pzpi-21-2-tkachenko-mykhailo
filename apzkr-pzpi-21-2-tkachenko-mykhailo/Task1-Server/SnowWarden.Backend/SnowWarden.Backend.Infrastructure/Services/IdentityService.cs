using Microsoft.AspNetCore.Identity;

using SnowWarden.Backend.Core.Exceptions;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Identity.Services;

using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Services;
using SnowWarden.Backend.Core.Utils.Results;

namespace SnowWarden.Backend.Infrastructure.Services;

public class IdentityService<TUser> : IIdentityService<TUser> where TUser : ApplicationUser, new()
{
	private readonly IApplicationUserManager<TUser> _memberManager;

	public IdentityService(IApplicationUserManager<TUser> memberManager)
	{
		_memberManager = memberManager;
	}

	public async Task<ApplicationIdentityResult<TUser>> SignIn(string contact, string password)
	{
		ApplicationIdentityResult<TUser> searchIdentityResult = await FindAsync(contact);
		if (!searchIdentityResult.Succeeded)
			return ApplicationIdentityResult<TUser>.Failure(
				type: IdentityResultType.InvalidContact,
				errors: [
					new LocalizedContent
					{
						Translations = new LocalizationDictionary
						{
							{Localizator.SupportedLanguages.AmericanEnglish, "User provided incorrect credentials"},
							{Localizator.SupportedLanguages.Ukrainian, "Користувач ввів неправильні дані"}
						}
					}
				]);
		TUser foundUser = searchIdentityResult.User!;

		bool isEmailConfirmed = foundUser.EmailConfirmed;
		bool isLockedOut = await _memberManager.IsLockedOutAsync(foundUser);
		bool passwordValid = await _memberManager.CheckPasswordAsync(foundUser, password);

		if (!isEmailConfirmed)
		{
			return ApplicationIdentityResult<TUser>.Failure(
				type: IdentityResultType.EmailNotConfirmed,
				errors: [
					new LocalizedContent
					{
						Translations = new LocalizationDictionary
						{
							{Localizator.SupportedLanguages.AmericanEnglish, $"User {contact} has not confirmed email yet" },
							{Localizator.SupportedLanguages.Ukrainian, $"Користувач {contact} ще не підтвердив свою пошту" }
						}
					}
				]);
		}

		if (isLockedOut)
		{
			return ApplicationIdentityResult<TUser>.Failure(
				type: IdentityResultType.LockedOut,
				errors: [
					new LocalizedContent
					{
						Translations = new LocalizationDictionary
						{
							{Localizator.SupportedLanguages.AmericanEnglish, $"User with contact {contact} exists, but it is being under lockout"},
							{Localizator.SupportedLanguages.Ukrainian, $"Користувач з контактною інформацією {contact} існує, але діє локдаун"}
						}
					}
				]);
		}

		if (!passwordValid)
		{
			return ApplicationIdentityResult<TUser>.Failure(
				type: IdentityResultType.InvalidPassword,
				errors: [
					new LocalizedContent
					{
						Translations = new LocalizationDictionary
						{
							{Localizator.SupportedLanguages.AmericanEnglish, "User provided incorrect credentials"},
							{Localizator.SupportedLanguages.Ukrainian, "Користувач ввів неправильні дані"}
						}
					}
				]);
		}

		return ApplicationIdentityResult<TUser>
			.Successful(
				type: IdentityResultType.Valid,
				user: foundUser);
	}
	public async Task<ApplicationIdentityResult<TUser>> Register(TUser user, string password)
	{
		IdentityResult identityResult = await _memberManager.CreateAsync(user, password);

		if (!identityResult.Succeeded)
		{
			ApplicationIdentityResult<TUser>
				.Failure(
					IdentityResultType.Invalid,
					errors:
					[
						new LocalizedContent
						{
							Translations = new LocalizationDictionary
							{
								{
									Localizator.SupportedLanguages.AmericanEnglish,
									"User provided incorrect credentials"
								},
								{
									Localizator.SupportedLanguages.Ukrainian,
									"Користувач ввів неправильні дані"
								}
							}
						}
					]);
		}

		TUser registered = await _memberManager.FindByEmailAsync(user.Email ?? string.Empty) ?? throw new LocalizedException(
			new LocalizedContent
			{
				Translations = new LocalizationDictionary
				{
					{
						Localizator.SupportedLanguages.AmericanEnglish,
						"Registration operation has failed, try again"
					},
					{
						Localizator.SupportedLanguages.Ukrainian,
						"Не вдалось зареєструвати користувача, спробуйте ще раз"
					}
				}
			});

		return ApplicationIdentityResult<TUser>.Successful(
			type: IdentityResultType.Valid,
			user: registered);
	}

	public async Task<ApplicationIdentityResult<TUser>> RegisterWithTemporaryPassword(TUser user)
	{
		IdentityResult result = await _memberManager.CreateWithTemporaryPasswordAsync(user, 8);
		LocalizedException exception = new(
			new LocalizedContent
			{
				Translations = new LocalizationDictionary
				{
					{
						Localizator.SupportedLanguages.AmericanEnglish,
						"Registration operation has failed, try again"
					},
					{
						Localizator.SupportedLanguages.Ukrainian,
						"Не вдалось зареєструвати користувача, спробуйте ще раз"
					}
				}
			});

		if (!result.Succeeded) throw exception;

		TUser registered = await _memberManager.FindByEmailAsync(user.Email ?? string.Empty) ?? throw exception;

		return ApplicationIdentityResult<TUser>.Successful(
			type: IdentityResultType.Valid,
			user: registered);
	}

	public async Task<ApplicationIdentityResult<TUser>> ConfirmEmail(int userId, string token)
	{
		TUser? foundGuest = await _memberManager.FindByIdAsync(userId.ToString());

		if (foundGuest is null)
		{
			return ApplicationIdentityResult<TUser>.Failure(IdentityResultType.InvalidContact,
				errors: [
					new LocalizedContent
					{
						Translations = new LocalizationDictionary
						{
							{Localizator.SupportedLanguages.AmericanEnglish, "User was not found"},
							{Localizator.SupportedLanguages.Ukrainian, "Користувача не знайдено"}
						}
					}
				]);
		}

		IdentityResult identityResult = await _memberManager.ConfirmEmailAsync(foundGuest, token);
		if (!identityResult.Succeeded)
		{
			return ApplicationIdentityResult<TUser>
				.Failure(
					type: IdentityResultType.Invalid,
					errors:
					[
						new LocalizedContent
						{
							Translations = new LocalizationDictionary
							{
								{
									Localizator.SupportedLanguages.AmericanEnglish,
									"User provided incorrect token or id"
								},
								{ Localizator.SupportedLanguages.Ukrainian, "Користувач ввів неправильний токен або ідентифікатор" }
							}
						}
					]);
		}

		return ApplicationIdentityResult<TUser>.Successful(IdentityResultType.Valid);
	}
	public async Task<ApplicationIdentityResult<TUser>> UpdateAsync(TUser user)
	{
		TUser? foundUser = await _memberManager.FindByIdAsync(user.Id.ToString());
		if (foundUser is null)
		{
			return ApplicationIdentityResult<TUser>.Failure(IdentityResultType.InvalidContact, errors:
			[
				new LocalizedContent
				{
					Translations = new LocalizationDictionary
					{
						{ Localizator.SupportedLanguages.AmericanEnglish, "Invalid user for update" },
						{
							Localizator.SupportedLanguages.Ukrainian,
							"Не вдалось оновити дані користувача, спробуйте ще раз"
						}
					}
				}
			]);
		}

		await _memberManager.UpdateAsync(foundUser);

		return ApplicationIdentityResult<TUser>.Successful(IdentityResultType.Valid, foundUser);
	}
	public async Task<ApplicationIdentityResult<TUser>> FindAsync(string contact)
	{
		TUser? foundUser = await _memberManager.FindByEmailAsync(contact) ?? await _memberManager.FindByNameAsync(contact);

		if (foundUser is null)
		{
			return ApplicationIdentityResult<TUser>.Failure(
				type: IdentityResultType.InvalidContact,
				errors: [
					new LocalizedContent
					{
						Translations = new LocalizationDictionary
						{
							{Localizator.SupportedLanguages.AmericanEnglish, $"User with contact {contact} does not exist"},
							{Localizator.SupportedLanguages.Ukrainian, $"Користувач з контактною інформацією {contact} не існує"}
						}
					}
				]);
		}

		return ApplicationIdentityResult<TUser>.Successful(IdentityResultType.Valid, foundUser);
	}
}