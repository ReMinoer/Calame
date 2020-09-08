using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Calame.Converters
{
    public class BooleanOrBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            values = values.Select(x => x == DependencyProperty.UnsetValue ? false : x).ToArray();

            bool value = (bool)values[0] || (bool)values[1];
            if (parameter != null)
                value = !value;

            return value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new [] {Binding.DoNothing, Binding.DoNothing};
        }
    }
}