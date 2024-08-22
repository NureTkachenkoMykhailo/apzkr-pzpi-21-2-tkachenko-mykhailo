using Microsoft.AspNetCore.Identity;
using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Abstractions.Events;
using SnowWarden.Backend.Core.Features.Identity.Events;
using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Core.Features.Identity;

public class ApplicationUser : IdentityUser<int>, IDbEntity, IEventSource
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string LanguageCode { get; private set; }

	public Language Language { get; private set; }

	public override string? Email
	{
		get => base.Email;
		set
		{
			base.Email = value;
			UserName = value;
		}
	}

	public void SetLanguage(string code)
	{
		Language language = Localizator.SupportedLanguages.LanguageResolver.GetLanguage(code);

		LanguageCode = language.Code;
		Language = language;
	}

	public void SetExistingId(int id) => Id = id;

	protected readonly List<IDomainEvent> DomainEvents = [];

	public IReadOnlyCollection<IDomainEvent> GetEvents()
	{
		List<IDomainEvent> events = DomainEvents.ToList();
		DomainEvents.Clear();
		return events;
	}

	public void RaiseUserRegisteredEvent(string? tempPassword = null)
	{
		DomainEvents.Add(new UserRegisteredSuccessfullyDomainEvent(this, tempPassword));
	}

	public void RaiseUserEmailConfirmedEvent()
	{
		DomainEvents.Add(new UserEmailConfirmedEvent(this));
	}
}