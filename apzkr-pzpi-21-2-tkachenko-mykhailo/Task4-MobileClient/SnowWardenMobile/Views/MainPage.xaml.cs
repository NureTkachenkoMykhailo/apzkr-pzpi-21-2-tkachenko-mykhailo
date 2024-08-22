using SnowWardenMobile.Abstractions.Services;
using SnowWardenMobile.ViewModels;

namespace SnowWardenMobile.Views;

public partial class MainPage : ContentPage
{
	private readonly IAuthService _authService;
	private readonly MainPageViewModel _vm;

	public MainPage(IAuthService authService, MainPageViewModel viewModel)
	{
		_authService = authService;
		_vm = viewModel;

		InitializeComponent();

		BindingContext = viewModel;
	}

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		bool isNotAuthenticated = await _authService.IsAuthenticatedAsync() is false;
		if (isNotAuthenticated)
		{
			await Shell.Current.GoToAsync(Global.ApplicationRoutes.LoginPage);
		}
		else
		{
			await _vm.LoadTrainingSessionsAsync();
			base.OnNavigatedTo(args);
		}
	}
}