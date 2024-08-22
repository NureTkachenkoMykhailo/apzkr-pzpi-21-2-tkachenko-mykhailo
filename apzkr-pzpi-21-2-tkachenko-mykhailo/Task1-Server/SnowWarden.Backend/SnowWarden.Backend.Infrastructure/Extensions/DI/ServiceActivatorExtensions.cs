using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Abstractions.Events;

using SnowWarden.Backend.Core.Features.Booking;
using SnowWarden.Backend.Core.Features.Identity.Services;
using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Core.Features.Track;
using SnowWarden.Backend.Core.Features.Training;

using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Messaging;
using SnowWarden.Backend.Infrastructure.Repositories;
using SnowWarden.Backend.Infrastructure.Services;

namespace SnowWarden.Backend.Infrastructure.Extensions.DI;

public static class ServiceActivatorExtensions
{
	public static IServiceCollection AddIdentityServices(this IServiceCollection services)
	{
		services.TryAddScoped(typeof(IIdentityService<>), typeof(IdentityService<>));
		services.AddScoped(typeof(IApplicationUserManager<>), typeof(ApplicationUserManager<>));

		return services;
	}

	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
	{
		services.TryAddScoped<AttachMaster>();
		return services;
	}

	public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddIdentityServices();
		services.AddRepositories();
		services.AddInfrastructureServices();
		services.AddApplicationDatabase(configuration);
		services.AddEventBus();

		return services;
	}

	private static void AddRepositories(this IServiceCollection services)
	{
		services.TryAddScoped<IRepository<TrainingSession>, TrainingRepository>();
		services.TryAddScoped<IRepository<Section>, TrackSectionRepository>();
		services.TryAddScoped<IRepository<Track>, TrackRepository>();
		services.TryAddScoped<IRepository<Booking>, BookingRepository>();
		services.TryAddScoped<IRepository<Inventory>, InventoryRepository>();
		services.TryAddScoped<IRepository<InventoryItem>, InventoryItemRepository>();
	}

	public static IServiceCollection AddApplicationDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>(opt =>
		{
			opt.UseNpgsql(configuration.GetConnectionString("Postgres"));
		});

		return services;
	}

	public static IServiceCollection AddEventBus(this IServiceCollection services)
	{
		services.AddSingleton<EventChannel>();
		services.AddSingleton<IEventBus, EventBus>();

		return services;
	}
}