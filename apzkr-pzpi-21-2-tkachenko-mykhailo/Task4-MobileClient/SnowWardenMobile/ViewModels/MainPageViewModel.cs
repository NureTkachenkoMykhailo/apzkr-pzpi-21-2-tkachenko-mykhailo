using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SnowWardenMobile.Abstractions.Services;
using SnowWardenMobile.Models.Trainings;
using SnowWardenMobile.Services;

namespace SnowWardenMobile.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
	private readonly ITrainingSessionService _trainingService;

	public ObservableCollection<TrainingSession> TrainingSessions { get; } = [];

	public MainPageViewModel() { }
	public MainPageViewModel(TrainingSessionService trainingService)
	{
		_trainingService = trainingService;
		LoadTrainingSessionsCommand = new AsyncRelayCommand(LoadTrainingSessionsAsync);
	}

	public IAsyncRelayCommand LoadTrainingSessionsCommand { get; }

	public async Task LoadTrainingSessionsAsync()
	{
		try
		{
			ICollection<TrainingSession> sessions = await _trainingService.GetTrainingSessionsAsync();
			if (TrainingSessions.Any())
			{
				sessions = TrainingSessions
					.Where(ts =>
						sessions
							.Select(s => s.Id)
							.Contains(ts.Id) is false)
					.ToList();
			}

			foreach (TrainingSession session in sessions)
			{
				TrainingSessions.Add(session);
			}
		}
		catch (TrainingSessionService.GetTrainingSessionsRequestFailedException ex)
		{
			await Application.Current.MainPage.DisplayAlert($"Fetch error", ex.Message, "OK");
		}
	}

	[RelayCommand]
	private async Task ViewTrainingDetailsAsync(TrainingSession? selectedSession)
	{
		if (selectedSession is not null)
		{
			Dictionary<string, object> navigationParameter = new()
			{
				{ "SelectedSession", selectedSession }
			};

			await Shell.Current.GoToAsync(Global.ApplicationRoutes.TrainingDetailsPage, navigationParameter);
		}
	}
}