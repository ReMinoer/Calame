using System;
using System.Globalization;
using System.Windows.Data;
using Diese;

namespace Calame.Converters
{
    public class DisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Null";

            string toString = value.ToString();
            Type type = value.GetType();
            if (toString == type.ToString())
                return type.GetDisplayName();

            return toString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}