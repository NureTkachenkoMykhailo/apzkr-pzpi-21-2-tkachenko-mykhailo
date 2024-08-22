using SnowWarden.Backend.Core.Exceptions;
using SnowWarden.Backend.Core.Utils;
using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Infrastructure.Exceptions;

public class InfrastructureException(LocalizedContent message) : LocalizedException(message);