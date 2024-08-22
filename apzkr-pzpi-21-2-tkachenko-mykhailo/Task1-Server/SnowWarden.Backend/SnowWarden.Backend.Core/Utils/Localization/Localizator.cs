using SnowWarden.Backend.Core.Utils.Localization.Services;

namespace SnowWarden.Backend.Core.Utils.Localization;

public static class Localizator
{
	public static class SupportedLanguages
	{
		public static readonly Language AmericanEnglish = new("en", "American english");
		public static readonly Language Ukrainian = new("ua", "Українська мова");

		public static readonly LanguageDictionary LanguageResolver = new()
		{
			{ AmericanEnglish.Code, AmericanEnglish},
			{ Ukrainian.Code, Ukrainian }
		};
	}
}