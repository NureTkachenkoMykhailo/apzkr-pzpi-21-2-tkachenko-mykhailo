using Microsoft.EntityFrameworkCore;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Services;

namespace SnowWarden.Backend.Infrastructure.Repositories;

public class TrackSectionRepository(ApplicationDbContext context, AttachMaster attachMaster) : RepositoryBase<Section>(context, attachMaster)
{
	protected override async Task UpdateInternalAsync(Section source, Section compare)
	{
		await ValidateSingleAttachment(source, e => e.Track, e => e.TrackId);
		//source.TrackId = source.Track.Id;
	}

	protected override async Task CreateInternalAsync(Section addedEntity)
	{
		await ValidateSingleAttachment(addedEntity, e => e.Track, e => e.TrackId);
		//addedEntity.TrackId = addedEntity.Track.Id;
	}

	protected override IQueryable<Section> IncludeComplete(DbSet<Section> set)
	{
		return set.Include(s => s.Track);
	}

	protected override IQueryable<Section> IncludeLightweight(DbSet<Section> set)
	{
		return set;
	}
}