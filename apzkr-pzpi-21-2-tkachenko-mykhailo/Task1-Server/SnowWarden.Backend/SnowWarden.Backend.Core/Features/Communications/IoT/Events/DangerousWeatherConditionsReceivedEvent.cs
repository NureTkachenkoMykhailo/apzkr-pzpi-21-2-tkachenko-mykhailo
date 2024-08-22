using SnowWarden.Backend.Core.Abstractions.Events;

namespace SnowWarden.Backend.Core.Features.Communications.IoT.Events;

public class DangerousWeatherConditionsReceivedEvent(IoTMonitoringDeviceMessage data) : DomainEvent
{
	public override int TimeoutSeconds => 10;
	public IoTMonitoringDeviceMessage Data { get; init; } = data;
}