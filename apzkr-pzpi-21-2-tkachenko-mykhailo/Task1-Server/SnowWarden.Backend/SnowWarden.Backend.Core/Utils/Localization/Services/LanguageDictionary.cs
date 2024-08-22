using SnowWarden.Backend.Core.Exceptions;

namespace SnowWarden.Backend.Core.Utils.Localization.Services;

public class LanguageDictionary : Dictionary<string, Language>
{
	public Language GetLanguage(string code)
	{
		if (TryGetValue(code, out Language language))
		{
			return language;
		}

		throw new InvalidLocalizationException(code);
	}
}