using System.Globalization;
using System.Windows.Data;

namespace TournamentDJ.Converter
{

	class DateTimeToHasBeenPlayedConverter : IValueConverter
	{
		/// <summary>
		/// Converts a DateTime do a boolean, indicating if it is today or not
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns>true, if DateTime is today, otherwise false</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			DateTime dateTimeToConvert = DateTime.Parse(value.ToString());
			TimeSpan timeDiff = DateTime.Now.Subtract(dateTimeToConvert);
			if(dateTimeToConvert.Year == DateTime.Now.Year && dateTimeToConvert.DayOfYear == DateTime.Now.DayOfYear)
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
