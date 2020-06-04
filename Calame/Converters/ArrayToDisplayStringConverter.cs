using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Simulacra.Utils;

namespace Calame.Converters
{
    public class ArrayToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return $"{string.Join("x", ((IArray)value).Lengths())} {value.GetType().GenericTypeArguments.FirstOrDefault()?.Name}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}