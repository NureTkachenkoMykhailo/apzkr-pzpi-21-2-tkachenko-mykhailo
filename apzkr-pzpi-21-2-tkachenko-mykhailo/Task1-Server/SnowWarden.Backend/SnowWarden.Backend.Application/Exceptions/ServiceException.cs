using SnowWarden.Backend.Core.Exceptions;
using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Application.Exceptions;

public class ServiceException(string serviceName, LocalizedContent message) : LocalizedException(message)
{
	public string ServiceName => serviceName;
}