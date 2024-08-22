using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SnowWarden.Backend.Application.Services;

using SnowWarden.Backend.Core.Features.Booking.Services;
using SnowWarden.Backend.Core.Features.Inventory.Services;
using SnowWarden.Backend.Core.Features.Members.Services;
using SnowWarden.Backend.Core.Features.Track.Services;
using SnowWarden.Backend.Core.Features.Training.Services;

namespace SnowWarden.Backend.Application.Extensions.DI;

public static class ServiceActivator
{
	public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
	{
		services.AddDataManagementServices();
		services.AddApplicationEventHandlers();

		return services;
	}

	private static IServiceCollection AddDataManagementServices(this IServiceCollection services)
	{
		services.TryAddScoped<ITrainingService, TrainingService>();
		services.TryAddScoped<ITrackService, TrackService>();
		services.TryAddScoped<IBookingService, BookingService>();
		services.TryAddScoped<IInstructorService, InstructorManagementService>();
		services.TryAddScoped<IInventoryService, InventoryService>();
		services.TryAddScoped<IInventoryItemService, InventoryItemService>();
		services.TryAddScoped<ITrackSectionService, TrackSectionService>();

		return services;
	}

	private static IServiceCollection AddApplicationEventHandlers(this IServiceCollection services)
	{
		services.AddMediatR(opt =>
			opt.RegisterServicesFromAssemblies(typeof(ServiceActivator).Assembly));
		return services;
	}
}