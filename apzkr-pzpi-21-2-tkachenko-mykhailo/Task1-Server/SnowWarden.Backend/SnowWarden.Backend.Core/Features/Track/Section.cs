using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Features.Communications.IoT;

namespace SnowWarden.Backend.Core.Features.Track;

public class Section : IDbEntity
{
	public int Id { get; private set; }
	public int TrackId { get; init; }
	public SectionInfo Information { get; init; }
	public ICollection<IoTMonitoringDeviceLog> IotLogs { get; private set; }

	public SectionFactors DeterminingFactors { get; init; } = new();

	public Track Track { get; init; }

	private int _aggregateDangerIndex;

	public int AggregateDangerIndex
	{
		get => _aggregateDangerIndex;
		private set => _aggregateDangerIndex =
			value > (int)Information.Danger
				? (int)Adapt(value)
				: (int)Information.Danger;
	}

	public void SetDanger(DangerLevel danger)
	{
		Information.Danger = danger;
		AggregateDangerIndex = (int)danger;
	}

	public void SetExistingId(int id) => Id = id;

	public void RecalculateDangerIndex(IotRequestMetadata metadata)
	{
		int sum = _aggregateDangerIndex;
		foreach (SectionFactor sectionFactor in DeterminingFactors.Values)
		{
			int value = metadata.GetFactorValue(sectionFactor.Key);
			if (value > (int)DangerLevel.Mediocre)
			{
				sum += (int)Math.Ceiling(value * sectionFactor.MultiplicationValue);
			}
		}

		AggregateDangerIndex = (int)Math.Ceiling((double)sum);
	}

	public void SaveLog(IoTMonitoringDeviceMessage message)
	{
		IotLogs ??= [];
		IotLogs.Add(new IoTMonitoringDeviceLog(message));
	}

	private DangerLevel Adapt(int dangerValue)
	{
		return dangerValue switch
		{
			<= (int)DangerLevel.NoDanger => DangerLevel.NoDanger,
			> (int)DangerLevel.NoDanger and <= (int)DangerLevel.Mediocre => DangerLevel.Mediocre,
			> (int)DangerLevel.Mediocre and <= (int)DangerLevel.Dangerous => DangerLevel.Dangerous,
			_ => dangerValue is > (int)DangerLevel.Dangerous and <= (int)DangerLevel.VeryDangerous
				? DangerLevel.VeryDangerous
				: DangerLevel.ExceptionalDanger
		};
	}
}

public class SectionFactors : Dictionary<string, SectionFactor>;