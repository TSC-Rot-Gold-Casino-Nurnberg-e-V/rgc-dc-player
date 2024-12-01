using System.Globalization;
using System.Windows.Data;

namespace TournamentDJ.Converter
{
	class DateTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) { return String.Empty; }
			string path = value.ToString();
			path = path.Remove(0, 8);
			return path;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

	}
}
