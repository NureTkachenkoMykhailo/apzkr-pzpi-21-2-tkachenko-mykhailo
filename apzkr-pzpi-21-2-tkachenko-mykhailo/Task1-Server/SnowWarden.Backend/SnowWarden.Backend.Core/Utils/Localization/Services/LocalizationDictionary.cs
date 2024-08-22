using SnowWarden.Backend.Core.Exceptions;

namespace SnowWarden.Backend.Core.Utils.Localization.Services;

public class LocalizationDictionary : Dictionary<Language, string>
{
	public string GetWithLanguage(Language language)
	{
		if (TryGetValue(language, out string text))
		{
			return text;
		}

		throw new InvalidLocalizationException(language.Code);
	}
}