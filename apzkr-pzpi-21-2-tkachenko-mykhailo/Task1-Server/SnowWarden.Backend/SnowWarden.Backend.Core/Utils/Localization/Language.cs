namespace SnowWarden.Backend.Core.Utils.Localization;

public class Language
{
	public Language(string code, string representation)
	{
		Code = code;
		Representation = representation;
	}

	public Language(){ }

	public string Code { get; init; }

	public string Representation { get; init; }

	public override string ToString() => Representation;
}