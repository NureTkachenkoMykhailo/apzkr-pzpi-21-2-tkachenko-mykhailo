namespace SnowWarden.Backend.Core.Features.Communications.IoT;

public class IotRequestMetadata : Dictionary<string, int>
{
	public int GetFactorValue(string factor)
	{
		TryGetValue(factor, out int factorValue);

		return factorValue;
	}
}