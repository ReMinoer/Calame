using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calame.Converters
{
    public class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool notCollapsed = parameter != null;
            return value != null ? Visibility.Visible : notCollapsed ? Visibility.Hidden : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}