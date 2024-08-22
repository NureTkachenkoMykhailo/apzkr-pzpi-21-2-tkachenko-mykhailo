namespace SnowWarden.Backend.Core.Utils.Security;

public class PasswordGenerationProperties
{
	public bool IncludeUppercase { get; set; } = true;
	public bool IncludeLowercase { get; set; } = true;
	public bool IncludeDigits { get; set; } = true;
	public bool IncludeSpecialSymbols { get; set; } = true;
}