using System.Text;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnowWarden.Backend.Core.Abstractions.Events;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Identity.Services;
using SnowWarden.Backend.Core.Utils.Security;

namespace SnowWarden.Backend.Infrastructure.Services;

public class ApplicationUserManager<TUser> : UserManager<TUser>, IApplicationUserManager<TUser> where TUser : ApplicationUser
{
	private readonly IEventBus _eventBus;

	public ApplicationUserManager(
		IUserStore<TUser> store,
		IOptions<IdentityOptions> optionsAccessor,
		IPasswordHasher<TUser> passwordHasher,
		IEnumerable<IUserValidator<TUser>> userValidators,
		IEnumerable<IPasswordValidator<TUser>> passwordValidators,
		ILookupNormalizer keyNormalizer,
		IdentityErrorDescriber errors,
		IServiceProvider services,
		ILogger<ApplicationUserManager<TUser>> logger, IEventBus eventBus)
		: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
	{
		_eventBus = eventBus;
	}


	public async Task<IdentityResult> CreateWithTemporaryPasswordAsync(TUser user, int passwordLength, PasswordGenerationProperties? properties = null)
	{
		properties ??= new PasswordGenerationProperties();
		string password = GenerateRandomPassword(passwordLength, properties);
		IdentityResult result = await base.CreateAsync(user, password);

		if (!result.Succeeded) return result;

		user.RaiseUserRegisteredEvent(password);
		await PublishEvents(user);

		return result;
	}

	public override async Task<IdentityResult> CreateAsync(TUser user, string password)
	{
		IdentityResult result = await base.CreateAsync(user, password);

		if (!result.Succeeded) return result;

		user.RaiseUserRegisteredEvent();
		await PublishEvents(user);

		return result;
	}

	public override async Task<IdentityResult> ConfirmEmailAsync(TUser user, string token)
	{
		IdentityResult result = await base.ConfirmEmailAsync(user, token);

		if (!result.Succeeded) return result;

		user.RaiseUserEmailConfirmedEvent();
		await PublishEvents(user);

		return result;
	}

	public async Task<string> GenerateEmailConfirmationTokenUrlQuery(TUser guest)
	{
		string token = await GenerateEmailConfirmationTokenAsync(guest);
		int userId = guest.Id;

		return $"userId={userId}&token={token}";
	}

	public async Task<IReadOnlyCollection<TUser>> GetUsers()
	{
		return await Users.ToListAsync();
	}

	private string GenerateRandomPassword(int length, PasswordGenerationProperties? properties = null)
	{
		properties ??= new PasswordGenerationProperties();
		if (length <= 0)
			throw new ArgumentException("Password length must be greater than zero.", nameof(length));

		string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		string lowercase = "abcdefghijklmnopqrstuvwxyz";
		string digits = "0123456789";
		string specialSymbols = "!@#$%^&*()-_=+[]{}|;:,.<>?/";

		StringBuilder characterSet = new();

		List<char> requiredCharacters = [];

		if (properties.IncludeUppercase)
		{
			characterSet.Append(uppercase);
			requiredCharacters.Add(uppercase[new Random().Next(uppercase.Length)]);
		}

		if (properties.IncludeLowercase)
		{
			characterSet.Append(lowercase);
			requiredCharacters.Add(lowercase[new Random().Next(lowercase.Length)]);
		}

		if (properties.IncludeDigits)
		{
			characterSet.Append(digits);
			requiredCharacters.Add(digits[new Random().Next(digits.Length)]);
		}

		if (properties.IncludeSpecialSymbols)
		{
			characterSet.Append(specialSymbols);
			requiredCharacters.Add(specialSymbols[new Random().Next(specialSymbols.Length)]);
		}

		if (characterSet.Length == 0)
			throw new ArgumentException("At least one character type must be included.");
		if (length < requiredCharacters.Count)
			throw new ArgumentException(
				$"Password length must be at least {requiredCharacters.Count} to include all specified character types.");

		StringBuilder password = new(length);

		foreach (char c in requiredCharacters)
		{
			password.Append(c);
		}

		for (int i = requiredCharacters.Count; i < length; i++)
		{
			int index = new Random().Next(characterSet.Length);
			password.Append(characterSet[index]);
		}

		return new string(password.ToString().OrderBy(c => new Random().Next()).ToArray());
	}

	private async Task PublishEvents(IEventSource eventSource)
	{
		List<IDomainEvent> events = eventSource.GetEvents().ToList();
		foreach (IDomainEvent @event in events)
		{
			await _eventBus.PublishAsync(@event);
		}
	}

}