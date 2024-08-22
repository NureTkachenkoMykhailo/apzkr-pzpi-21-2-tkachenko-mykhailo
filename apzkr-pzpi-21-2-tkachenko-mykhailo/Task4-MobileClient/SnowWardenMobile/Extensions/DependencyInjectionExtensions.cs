using SnowWardenMobile.Abstractions.Services;
using SnowWardenMobile.Services;

namespace SnowWardenMobile.Extensions;

public static class DependencyInjectionExtensions
{
	public static void AddTrainingSessionApiCallerService(this IServiceCollection services)
	{
		services.AddHttpClient<TrainingSessionService>(client =>
		{
			client.BaseAddress = new Uri(Global.API_BASE_URL);
		});
		services.AddTransient<ITrainingSessionService>(provider => provider.GetRequiredService<TrainingSessionService>());
	}

	public static void AddBookingApiCallerService(this IServiceCollection services)
	{
		services.AddHttpClient<BookingService>(client =>
		{
			client.BaseAddress = new Uri(Global.API_BASE_URL + "bookings/");
		});
		services.AddTransient<IBookingService>(provider => provider.GetRequiredService<BookingService>());
	}

	public static void AddClientAuthenticationServices(this IServiceCollection services)
	{
		services.AddHttpClient<AuthService>(client =>
		{
			client.BaseAddress = new Uri(Global.API_BASE_URL);
		});
		services.AddTransient<IAuthService>(provider => provider.GetRequiredService<AuthService>());
	}
}