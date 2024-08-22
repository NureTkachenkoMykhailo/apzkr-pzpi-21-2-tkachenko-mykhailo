using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Services;

namespace SnowWarden.Backend.Core.Exceptions;

public class InvalidLocalizationException(string language) : CoreException($"Language is not supported: {language}")
{
	public LocalizedContent LocalizedMessage = new()
	{
		Translations = new LocalizationDictionary
		{
			{ Localizator.SupportedLanguages.AmericanEnglish, $"Language is not supported: {language}" },
			{ Localizator.SupportedLanguages.Ukrainian, "Мова не підтримується" }
		}
	};
}