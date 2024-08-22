using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowWardenMobile.Abstractions.Services;
using SnowWardenMobile.Models;
using SnowWardenMobile.Models.Trainings;
using SnowWardenMobile.Services;

namespace SnowWardenMobile.ViewModels;

[QueryProperty(nameof(SelectedSession), "SelectedSession")]
public partial class TrainingDetailsViewModel : ObservableObject
{
	private readonly IBookingService _bookingService;
	public TrainingDetailsViewModel() { }
	public TrainingDetailsViewModel(IBookingService bookingService)
	{
		_bookingService = bookingService;
	}

	[ObservableProperty]
	private TrainingSession? _selectedSession;

	[ObservableProperty]
	private bool isBookButtonEnabled;

	[ObservableProperty]
	private string bookingButtonText;

	[ObservableProperty]
	private string bookingStatusMessage;

	[RelayCommand]
	private async Task BookTrainingAsync()
	{
		try
		{
			Booking result = await _bookingService.Create(SelectedSession?.Id ??
				throw new NullReferenceException("Could not read id of the selected training session"));
			await Application.Current.MainPage.DisplayAlert("Success", $"You have created booking, booking reference id is {result.Id}", "Thank you");
			await LoadBookingStatusAsync();
		}
		catch (BookingService.BookingRequestFailedException ex)
		{
			await Application.Current.MainPage.DisplayAlert("Fetch error", ex.Message, "OK");
		}
		catch (BookingService.BookingAuthorizationException ex)
		{
			await Shell.Current.GoToAsync(Global.ApplicationRoutes.LoginPage);
		}
	}

	public async Task<bool> IsSessionBooked()
	{
		try
		{
			ICollection<Booking> allBookings = await _bookingService.GetBookings();
			return allBookings.Any(b => b.TrainingId == SelectedSession?.Id);
		}
		catch (BookingService.BookingRequestFailedException ex)
		{
			await Application.Current.MainPage.DisplayAlert("Fetch error", ex.Message, "OK");
		}
		catch (BookingService.BookingAuthorizationException ex)
		{
			await Shell.Current.GoToAsync(Global.ApplicationRoutes.LoginPage);
			//await Application.Current.MainPage.DisplayAlert("Authorization error", ex.Message, "OK");
		}

		return false;
	}

	public async Task LoadBookingStatusAsync()
	{
		bool isBooked = await IsSessionBooked();
		IsBookButtonEnabled = !isBooked;
		BookingButtonText = isBooked ? "Already Booked" : "Book Training";
		BookingStatusMessage = isBooked ? "This session is already booked." : string.Empty;
	}

	[RelayCommand]
	private async Task GoBackAsync()
	{
		await Shell.Current.GoToAsync("..");
	}
}