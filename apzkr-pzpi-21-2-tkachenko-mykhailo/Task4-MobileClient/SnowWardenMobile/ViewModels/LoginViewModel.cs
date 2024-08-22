using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowWardenMobile.Services;
using SnowWardenMobile.Views;

namespace SnowWardenMobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
	private readonly AuthService _authService;

	[ObservableProperty]
	private string contact;

	[ObservableProperty]
	private string password;

	public LoginViewModel(AuthService authService)
	{
		_authService = authService;
	}

	public LoginViewModel() { }

	[RelayCommand]
	private async Task LoginAsync()
	{
		try
		{
			bool successfulLogin = await _authService.AuthenticateAsync(Contact, Password);
			if (successfulLogin)
			{
				await Shell.Current.GoToAsync(Global.ApplicationRoutes.MainPage);
			}
			else
			{
				await Application.Current.MainPage.DisplayAlert("Login Failed", "Invalid credentials", "OK");
			}
		}
		catch (AuthService.LoginRequestFailedException)
		{
			await Application.Current.MainPage.DisplayAlert("Login Failed",
				"Identity server is unavailable, try again later", "OK");
		}
		catch (Exception ex)
		{
			await Application.Current.MainPage.DisplayAlert("Application error",
				ex.Message, "OK");
		}
	}

	[RelayCommand]
	private async Task GoToRegisterPageAsync()
	{
		await Shell.Current.GoToAsync("//RegisterPage");
	}
}