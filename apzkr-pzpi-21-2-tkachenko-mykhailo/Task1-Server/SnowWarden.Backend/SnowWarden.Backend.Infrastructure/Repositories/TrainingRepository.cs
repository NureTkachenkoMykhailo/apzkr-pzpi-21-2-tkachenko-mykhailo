using Microsoft.EntityFrameworkCore;

using SnowWarden.Backend.Core.Features.Training;

using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Services;

namespace SnowWarden.Backend.Infrastructure.Repositories;

public class TrainingRepository : RepositoryBase<TrainingSession>
{
	public TrainingRepository(ApplicationDbContext context, AttachMaster attachMaster)
		: base(context, attachMaster) { }

	protected override async Task UpdateInternalAsync(TrainingSession source, TrainingSession compare)
	{
		await ValidateSingleAttachment(compare, s => s.Instructor, s => s.InstructorId);
		await ValidateSingleAttachment(compare, s => s.Track, s => s.TrackId);

		await AttachMaster.Attach(source, compare, s => s.InventoryItemsTook);
		await AttachMaster.Detach(source, compare, s => s.InventoryItemsTook);

		source.GeneralInformation = compare.GeneralInformation;

		//DetachNavigations(source);
	}

	protected override async Task CreateInternalAsync(TrainingSession entity)
	{
		await ValidateSingleAttachment(entity, s => s.Instructor, s => s.InstructorId);
		await ValidateSingleAttachment(entity, s => s.Track, s => s.TrackId);

		await AttachMaster.Attach(entity, null, s => s.InventoryItemsTook);

		//DetachNavigations(entity);
	}

	// private void DetachNavigations(TrainingSession training)
	// {
	// 	training.InstructorId = training.Instructor.Id;
	// 	training.TrackId = training.Track.Id;
	// 	Context.Entry(training.Instructor).State = EntityState.Detached;
	// 	Context.Entry(training.Track).State = EntityState.Detached;
	// 	_ = training.Bookings?.Select(b =>
	// 	{
	// 		Context.Entry(b).State = EntityState.Detached;
	//
	// 		return b;
	// 	});
	//
	// 	training.Instructor = null;
	// 	training.Track = null;
	// }

	protected override IQueryable<TrainingSession> IncludeComplete(DbSet<TrainingSession> set)
	{
		return set
			.Include(t => t.Instructor)
			.Include(t => t.Track)
			.Include(t => t.InventoryItemsTook)
			.Include(t => t.Bookings)!
			.ThenInclude(b => b.Guest);
	}
	protected override IQueryable<TrainingSession> IncludeLightweight(DbSet<TrainingSession> set)
	{
		return set
			.Include(t => t.Instructor)
			.Include(t => t.Track);
	}
}