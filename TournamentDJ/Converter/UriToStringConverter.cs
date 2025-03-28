using System.Globalization;
using System.Windows.Data;

namespace TournamentDJ.Converter
{
    class UriToStringConverter : IValueConverter
    {
        /// <summary>
        /// Removes the first few bytes of a URI to make it easier to read as a path
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
