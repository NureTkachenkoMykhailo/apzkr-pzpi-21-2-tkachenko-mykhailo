using SnowWardenMobile.ViewModels;

namespace SnowWardenMobile.Views;

public partial class Login : ContentPage
{
	public Login(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}