using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Diese.Collections;

namespace Calame.Utils
{
    public class MoreThanOneToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is IEnumerable enumerable && enumerable.AtLeast(2) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}