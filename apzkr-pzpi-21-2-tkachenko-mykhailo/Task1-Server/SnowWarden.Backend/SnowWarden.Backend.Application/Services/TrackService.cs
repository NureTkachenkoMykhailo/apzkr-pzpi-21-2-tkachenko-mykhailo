using SnowWarden.Backend.Core.Abstractions;

using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Core.Features.Track.Services;

namespace SnowWarden.Backend.Application.Services;

public class TrackService(IRepository<Track> repository) : ServiceBase<Track>(repository), ITrackService;