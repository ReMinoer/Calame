using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calame.Converters
{
    public class RevertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;

            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            return (Visibility)value == Visibility.Collapsed;
        }
    }
}