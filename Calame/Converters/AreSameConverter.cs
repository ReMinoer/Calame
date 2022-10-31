using System;
using System.Globalization;
using System.Windows.Data;

namespace Calame.Converters
{
    public class AreSameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] == values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new [] { Binding.DoNothing, Binding.DoNothing };
        }
    }
}