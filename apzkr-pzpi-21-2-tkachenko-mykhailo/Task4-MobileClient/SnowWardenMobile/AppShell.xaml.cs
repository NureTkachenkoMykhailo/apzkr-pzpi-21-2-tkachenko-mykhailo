using SnowWardenMobile.Views;

namespace SnowWardenMobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(Login), typeof(Login));
		Routing.RegisterRoute(nameof(TrainingDetails), typeof(TrainingDetails));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
	}
}