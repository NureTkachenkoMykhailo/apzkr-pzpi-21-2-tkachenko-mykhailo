using Microsoft.Maui.Controls;

namespace SnowWardenMobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}
}