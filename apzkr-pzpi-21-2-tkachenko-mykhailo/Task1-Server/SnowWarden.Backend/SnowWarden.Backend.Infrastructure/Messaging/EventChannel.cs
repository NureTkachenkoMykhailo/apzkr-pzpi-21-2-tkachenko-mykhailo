using System.Threading.Channels;
using SnowWarden.Backend.Core.Abstractions.Events;

namespace SnowWarden.Backend.Infrastructure.Messaging;

public class EventChannel
{
	private readonly Channel<IEvent> _channel =
		Channel.CreateUnbounded<IEvent>();

	public ChannelReader<IEvent> Reader => _channel.Reader;

	public ChannelWriter<IEvent> Writer => _channel.Writer;
}