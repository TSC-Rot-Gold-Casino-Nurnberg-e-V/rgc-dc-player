using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TournamentDJ.Model;

namespace TournamentDJ.Converter
{
    public class IsSameTrackConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Track firstTrack = (Track)values[0];
            Track secondTrack = (Track)values[1];

            return firstTrack.Id == secondTrack.Id;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Going back to what you had isn't supported.");
        }
    }
}