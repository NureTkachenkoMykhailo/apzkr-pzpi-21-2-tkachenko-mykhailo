using SnowWardenMobile.ViewModels;

namespace SnowWardenMobile.Views;

public partial class TrainingDetails : ContentPage
{
	private readonly TrainingDetailsViewModel _vm;
	public TrainingDetails(TrainingDetailsViewModel vm)
	{
		_vm = vm;

		InitializeComponent();

		BindingContext = vm;
	}

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		await _vm.LoadBookingStatusAsync();
		base.OnNavigatedTo(args);
	}
}