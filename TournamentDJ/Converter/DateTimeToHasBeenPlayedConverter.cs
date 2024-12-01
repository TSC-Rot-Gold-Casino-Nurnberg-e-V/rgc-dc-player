using System.Globalization;
using System.Windows.Data;

namespace TournamentDJ.Converter
{
	class DateTimeToHasBeenPlayedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			DateTime dateTimeToConvert = DateTime.Parse(value.ToString());
			TimeSpan timeDiff = DateTime.Now.Subtract(dateTimeToConvert);
			if(timeDiff.TotalSeconds < 86400)
			{
				return true;
			}
			else 
			{ 
				return false;
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

	}
}
