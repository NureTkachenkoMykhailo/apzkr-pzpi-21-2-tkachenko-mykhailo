namespace SnowWarden.Backend.Core.Features.Communications.IoT;

public class IoTMonitoringDeviceMessage
{
	public int TrackSectionId { get; init; }
	public IotMessageType MessageType { get; init; }
	public string Message { get; init; } = string.Empty;
	public IotRequestMetadata? Metadata { get; init; }
}

// {
// 'message': 'weather conditions warning',
// 'metadata': {'iciness': 10, 'snow': 10, 'wind': 30},
// 'trackSectionId': 5,
// 'messageType': 2
// }