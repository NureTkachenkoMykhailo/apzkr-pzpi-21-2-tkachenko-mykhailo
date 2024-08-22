using Microsoft.AspNetCore.Mvc;

using SnowWarden.Backend.API.Attributes;
using SnowWarden.Backend.Core.Abstractions.Events;
using SnowWarden.Backend.Core.Features.Communications.IoT;
using SnowWarden.Backend.Core.Features.Communications.IoT.Events;

namespace SnowWarden.Backend.API.Controllers.Communications;

[Route("communications/iot")]
public class IotCommunicationController(
	IEventBus eventBus,
	ILogger<IotCommunicationController> logger) : ControllerBase
{
	[Route("")]
	[HttpPost]
	[RequireCommunicationSecret]
	public async Task<IActionResult> ReceiveData([FromBody] IoTMonitoringDeviceMessage iotData)
	{
		logger.LogInformation("Iot device communication is open");

		if (iotData.MessageType > IotMessageType.Monitor)
		{
			await eventBus.PublishAsync(new DangerousWeatherConditionsReceivedEvent(iotData));
		}

		logger.LogInformation("Iot device communication closed");

		return Ok();
	}
}