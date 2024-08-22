using Microsoft.EntityFrameworkCore;

using SnowWarden.Backend.Core.Features.Booking;

using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Services;

namespace SnowWarden.Backend.Infrastructure.Repositories;

public class BookingRepository : RepositoryBase<Booking>
{
	public BookingRepository(ApplicationDbContext context, AttachMaster attachMaster)
		: base(context, attachMaster) { }

	protected override async Task UpdateInternalAsync(Booking source, Booking compare)
	{
		await ValidateSingleAttachment(compare, b => b.TrainingSession, b => b.TrainingId);
		await ValidateSingleAttachment(compare, b => b.Guest, b => b.GuestId);
	}

	protected override async Task CreateInternalAsync(Booking booking)
	{
		await ValidateSingleAttachment(booking, b => b.Guest, b => b.GuestId);
		await ValidateSingleAttachment(booking, b => b.TrainingSession, b => b.TrainingId);
	}

	protected override IQueryable<Booking> IncludeComplete(DbSet<Booking> set)
	{
		return set.Include(b => b.Guest).Include(b => b.TrainingSession).ThenInclude(ts => ts.Instructor);
	}

	protected override IQueryable<Booking> IncludeLightweight(DbSet<Booking> set)
	{
		return IncludeComplete(set);
	}
}