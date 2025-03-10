﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TournamentDJ.Converter
{
    public class StringFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values,
            Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format((String)values[1], values[0]);
        }

        public object[] ConvertBack(object value,
            Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
