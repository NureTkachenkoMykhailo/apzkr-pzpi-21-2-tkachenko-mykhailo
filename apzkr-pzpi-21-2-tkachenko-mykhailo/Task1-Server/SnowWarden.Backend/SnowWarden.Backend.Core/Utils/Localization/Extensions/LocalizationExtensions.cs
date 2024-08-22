using SnowWarden.Backend.Core.Exceptions;

namespace SnowWarden.Backend.Core.Utils.Localization.Extensions;

public static class LocalizationExtensions
{
	public static string ToLocalizedString(this LocalizedContent content, Language language)
	{
		if (content.Translations.TryGetValue(language, out string? text))
		{
			return text;
		}

		throw new InvalidLocalizationException(language.Code);
	}

	public static string ToLocalizedString(this Exception ex, Language language)
	{
		if (ex is LocalizedException localizedException)
		{
			return localizedException.Message.ToLocalizedString(language);
		}

		return ex.Message;
	}
}