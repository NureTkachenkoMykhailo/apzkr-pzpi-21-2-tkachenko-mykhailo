namespace SnowWarden.Backend.Core.Features.Communications.IoT;

public class IoTMonitoringDeviceLog
{
	public IoTMonitoringDeviceLog() { }

	public IoTMonitoringDeviceLog(IoTMonitoringDeviceMessage message)
	{
		SectionId = message.TrackSectionId;
		IotMessage = message;
	}

	public int Id { get; set; }
	public int SectionId { get; set; }
	public IoTMonitoringDeviceMessage IotMessage { get; set; }
	public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}