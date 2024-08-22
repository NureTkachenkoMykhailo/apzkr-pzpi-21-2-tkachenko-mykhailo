using SnowWardenMobile.Views;

namespace SnowWardenMobile;

public static class Global
{
	public const string API_BASE_URL = "http://localhost:5298/";

	public static class ApplicationRoutes
	{
		public const string MainPage = $"{nameof(MainPage)}";
		public const string LoginPage = $"{nameof(Login)}";
		public const string TrainingDetailsPage = $"{nameof(TrainingDetails)}";
	}
}