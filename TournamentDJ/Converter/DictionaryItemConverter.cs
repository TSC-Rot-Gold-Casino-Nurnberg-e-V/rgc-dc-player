using System.Collections;
using System.Windows.Data;

namespace TournamentDJ.Converter
{
    public class DictionaryItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length >= 2)
            {
                var myDict = values[0] as IDictionary;
                var myKey = values[1].ToString();
                int id;
                int.TryParse(myKey, out id);
                if (myDict != null && id != null)
                {
                    return myDict[id].ToString();
                }
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}