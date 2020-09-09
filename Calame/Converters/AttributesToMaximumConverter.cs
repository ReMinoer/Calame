using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Data;
using Diese.Collections;

namespace Calame.Converters
{
    public class AttributesToMaximumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var attributes = (AttributeCollection)value;
            return attributes?.FirstOfTypeOrDefault<RangeAttribute>()?.Maximum;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}