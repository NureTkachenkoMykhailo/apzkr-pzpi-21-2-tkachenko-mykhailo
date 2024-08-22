using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Newtonsoft.Json;

using SnowWarden.Backend.Core.Abstractions.Events;
using SnowWarden.Backend.Core.Features.Booking;
using SnowWarden.Backend.Core.Features.Communications.IoT;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Core.Features.Training;

using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Infrastructure.Data;

public class ApplicationDbContext
	: IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
	private readonly IEventBus _eventBus;
	public DbSet<ApplicationUser> Identities { get; init; }
	public DbSet<Guest> Guests { get; init; }
	public DbSet<Instructor> Instructors { get; init; }
	public DbSet<Admin> Managers { get; init; }

	public DbSet<Inventory> Inventories { get; init; }
	public DbSet<InventoryItem> InventoryItems { get; init; }

	// public DbSet<Review> Reviews { get; init; }
	// public DbSet<Complaint> Complaints { get; init; }
	// public DbSet<Issue> Issues { get; init; }
	// public DbSet<Report> Reports { get; init; }

	public DbSet<Track> Tracks { get; init; }
	public DbSet<Section> Sections { get; init; }

	public DbSet<TrainingSession> Trainings { get; init; }
	public DbSet<Booking> Bookings { get; init; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEventBus eventBus) : base(options)
	{
		_eventBus = eventBus;
		ChangeTracker.Tracked += ChangeTracker_UserTracked;
	}

	private static void ChangeTracker_UserTracked(object? sender, EntityTrackedEventArgs e)
	{
		if (e.Entry.Entity is not ApplicationUser applicationUser || !e.FromQuery) return;
		applicationUser.SetLanguage(applicationUser.LanguageCode);
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<IdentityRole<int>>().HasKey(ar => ar.Id);
		builder.Entity<IdentityRole<int>>()
			.Property(ir => ir.Id)
			.UseIdentityByDefaultColumn();
		builder.Entity<ApplicationUser>().Ignore(a => a.Language);
		builder.Entity<ApplicationUser>().HasKey(au => au.Id);
		builder.Entity<ApplicationUser>()
			.Property(au => au.Id)
			.UseIdentityByDefaultColumn();
		builder.Entity<ApplicationUser>().UseTptMappingStrategy();

		builder.Entity<Guest>()
			.Property(gi => gi.Id)
			.UseIdentityByDefaultColumn();

		builder.Entity<Admin>()
			.Property(m => m.Id)
			.UseIdentityByDefaultColumn();

		builder.Entity<Instructor>()
			.Property(i => i.Id)
			.UseIdentityByDefaultColumn();
		builder.Entity<Instructor>().Ignore(i => i.ExperienceYears);

		builder.Entity<TrainingSession>().HasKey(ts => ts.Id);
		builder.Entity<TrainingSession>()
			.Property(ts => ts.Id)
			.UseIdentityByDefaultColumn();
		builder.Entity<TrainingSession>(training => training.OwnsOne(t => t.GeneralInformation, b => b.ToJson()));
		builder.Entity<TrainingSession>().OwnsMany(t => t.Levels, b => b.ToJson());
		builder.Entity<TrainingSession>()
			.HasOne(ts => ts.Instructor)
			.WithMany(i => i.Trainings)
			.HasForeignKey(ts => ts.InstructorId);
		builder.Entity<TrainingSession>()
			.HasOne(ts => ts.Track)
			.WithMany()
			.HasForeignKey(ts => ts.TrackId);
		builder.Entity<TrainingSession>()
			.HasMany(ts => ts.InventoryItemsTook)
			.WithMany(ii => ii.TrainingSessions)
			.UsingEntity<Dictionary<string, object>>(
				"TrainingSessionInventoryItem",
				e => e
					.HasOne<InventoryItem>()
					.WithMany()
					.HasForeignKey("InventoryItemId"),
				e => e
					.HasOne<TrainingSession>()
					.WithMany()
					.HasForeignKey("SessionId")
			);

		builder.Entity<Booking>().HasKey(s => s.Id);
		builder.Entity<Booking>().Property(s => s.Id).UseIdentityByDefaultColumn();
		builder.Entity<Booking>(booking =>
			booking
				.HasOne(b => b.TrainingSession)
				.WithMany(t => t.Bookings)
				.HasForeignKey(b => b.TrainingId));
		builder.Entity<Booking>(booking =>
			booking
				.HasOne(b => b.Guest)
				.WithMany(g => g.Bookings)
				.HasForeignKey(b => b.GuestId));

		builder.Entity<Section>().HasKey(s => s.Id);
		builder.Entity<Section>().Property(s => s.Id).UseIdentityByDefaultColumn();
		builder.Entity<Section>().OwnsOne(s => s.Information);
		builder.Entity<Section>().Property(s => s.DeterminingFactors)
			.HasConversion(
				v => JsonConvert.SerializeObject(v),
				v => JsonConvert.DeserializeObject<SectionFactors>(v) ?? new())
			.HasColumnType("json");

		builder.Entity<Language>().HasKey(l => new { l.Code });
		builder.Entity<Language>().HasData(
			Localizator.SupportedLanguages.AmericanEnglish,
			Localizator.SupportedLanguages.Ukrainian);

		builder.Entity<InventoryItem>().OwnsMany(ii => ii.Attributes, b =>
		{
			b.ToJson();
		});

		builder.Entity<IoTMonitoringDeviceLog>().HasKey(m => m.Id);
		builder.Entity<IoTMonitoringDeviceLog>().Property(m => m.Id).UseIdentityByDefaultColumn();
		builder.Entity<IoTMonitoringDeviceLog>().HasOne<Section>().WithMany().HasForeignKey(m => m.SectionId);
		builder.Entity<IoTMonitoringDeviceLog>().Property(log => log.IotMessage).HasConversion(
			to => JsonConvert.SerializeObject(to),
			from => JsonConvert.DeserializeObject<IoTMonitoringDeviceMessage>(from) ?? new());
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
	{
		IEnumerable<IEventSource> eventSources = ChangeTracker.Entries<IEventSource>().Select(e => e.Entity);
		List<IDomainEvent> domainEvents = eventSources.SelectMany(es => es.GetEvents()).ToList();

		int saveChanges = await base.SaveChangesAsync(cancellationToken);

		foreach (IDomainEvent @event in domainEvents)
		{
			await _eventBus.PublishAsync(@event, cancellationToken);
		}

		return saveChanges;
	}
}