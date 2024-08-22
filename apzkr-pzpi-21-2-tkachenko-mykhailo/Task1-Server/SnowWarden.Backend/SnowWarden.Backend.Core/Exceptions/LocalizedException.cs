using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Core.Exceptions;

public class LocalizedException(LocalizedContent message) : Exception
{
	public LocalizedContent Message { get; } = message;
}