using Microsoft.AspNetCore.Identity;

using SnowWarden.Backend.Application.Exceptions;
using SnowWarden.Backend.Core.Abstractions.Events;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Identity.Services;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Features.Members.Services;

using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Services;
using SnowWarden.Backend.Core.Utils.Results;

namespace SnowWarden.Backend.Application.Services;

public class InstructorManagementService(
	IIdentityService<Instructor> identityService,
	IApplicationUserManager<Instructor> instructorManager, IEventBus eventBus) : IInstructorService
{
	private readonly IEventBus _eventBus = eventBus;

	public async Task<ApplicationIdentityResult<Instructor>> CreateInstructorAccount(Instructor entity)
	{
		ApplicationIdentityResult<Instructor> createResult = await identityService
			.RegisterWithTemporaryPassword(entity);

		return createResult;
	}

	public async Task<ApplicationIdentityResult<Instructor>> DisableAsync(Instructor instructor, Admin admin)
	{
		IdentityResult result = await instructorManager.DeleteAsync(instructor);
		if (!result.Succeeded)
		{
			throw new ServiceException(
				GetType().Name,
				new LocalizedContent
				{
					Translations = new LocalizationDictionary
					{
						{ Localizator.SupportedLanguages.AmericanEnglish, "Could not delete instructor, try again later" },
						{ Localizator.SupportedLanguages.Ukrainian, "Не вийшло видалити інструктора, спробуйте пізніше" }
					}
				});

		}

		instructor.RaiseInstructorAccountDisabledEvent(admin);
		await PublishEvents(instructor);

		return ApplicationIdentityResult<Instructor>
			.Successful(IdentityResultType.Valid, instructor);
	}

	public async Task<ApplicationIdentityResult<Instructor>> GetByIdAsync(int id)
	{
		Instructor? instructor = await instructorManager.FindByIdAsync(id.ToString());

		return instructor is null
			? ApplicationIdentityResult<Instructor>.Failure(IdentityResultType.Invalid, user: instructor)
			: ApplicationIdentityResult<Instructor>.Successful(IdentityResultType.Valid, user: instructor);
	}

	public Task<IReadOnlyCollection<Instructor>> GetAsync()
	{
		return instructorManager.GetUsers();
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