using Microsoft.Extensions.Logging;

using SnowWardenMobile.Extensions;
using SnowWardenMobile.ViewModels;
using SnowWardenMobile.Views;

namespace SnowWardenMobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		MauiAppBuilder builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddClientAuthenticationServices();
		builder.Services.AddTrainingSessionApiCallerService();
		builder.Services.AddBookingApiCallerService();

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<MainPageViewModel>();

		builder.Services.AddTransient<Login>();
		builder.Services.AddTransient<LoginViewModel>();

		builder.Services.AddTransient<TrainingDetails>();
		builder.Services.AddTransient<TrainingDetailsViewModel>();

		builder.Logging.AddDebug();

		return builder.Build();
	}
}