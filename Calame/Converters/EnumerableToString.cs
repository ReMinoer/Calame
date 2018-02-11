using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Calame.Converters
{
    public class EnumerableToString : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string result = null;
            if (values[0] is IEnumerable<object> enumerable)
                result = string.Join(", ", enumerable.Select(x => x.ToString()));

            if (string.IsNullOrWhiteSpace(result))
                return "{empty}";

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new [] {Binding.DoNothing, Binding.DoNothing};
        }
    }
}