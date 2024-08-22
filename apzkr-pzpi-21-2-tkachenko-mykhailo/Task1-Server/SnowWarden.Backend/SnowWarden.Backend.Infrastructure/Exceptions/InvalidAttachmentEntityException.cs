using SnowWarden.Backend.Core.Abstractions;

using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Services;

namespace SnowWarden.Backend.Infrastructure.Exceptions;

public class InvalidAttachmentEntityException(IDbEntity attachment) : InfrastructureException(new LocalizedContent
{
	Translations = new LocalizationDictionary
	{
		{
			Localizator.SupportedLanguages.AmericanEnglish,
			$"Attachment {attachment.GetType().Name} with id {attachment.Id} does not exist"
		},
		{
			Localizator.SupportedLanguages.Ukrainian,
			$"Вкладена сутність {attachment.GetType().Name} з ідентифікатором {attachment.Id} не існує"
		}
	}
});