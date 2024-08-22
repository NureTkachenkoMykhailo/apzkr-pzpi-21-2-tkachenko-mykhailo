using Microsoft.EntityFrameworkCore;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Services;

namespace SnowWarden.Backend.Infrastructure.Repositories;

public class TrackRepository : RepositoryBase<Track>
{
	public TrackRepository(ApplicationDbContext context, AttachMaster attachMaster) : base(context, attachMaster)
	{
	}

	protected override async Task UpdateInternalAsync(Track source, Track compare)
	{
		await ValidateManyAttachments(source, t => t.Sections);

		await AttachMaster.Attach(source, compare, s => s.Sections, withCreate: true);
		await AttachMaster.Detach(source, compare, s => s.Sections, withDelete: true);
	}

	protected override Task CreateInternalAsync(Track addedEntity)
	{
		foreach (Section section in addedEntity.Sections)
		{
			section.SetDanger(section.Information.Danger);
		}
		return Task.CompletedTask;
	}

	protected override IQueryable<Track> IncludeComplete(DbSet<Track> set)
	{
		return set.Include(t => t.Sections);
	}

	protected override IQueryable<Track> IncludeLightweight(DbSet<Track> set)
	{
		return set;
	}
}