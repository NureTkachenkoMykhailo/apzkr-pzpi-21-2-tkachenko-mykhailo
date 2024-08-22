using System.Globalization;

namespace SnowWardenMobile.Utils.Converters;

public class InverseBooleanConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is bool boolean)
		{
			return !boolean;
		}
		return false;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}