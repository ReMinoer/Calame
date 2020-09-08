using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Calame.Converters
{
    public class EnumTypeToValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumType = (Type)value;
            if (enumType == null)
                return Array.Empty<object>();

            return enumType.GetFields()
                .Where(x => x.IsLiteral && (x.GetCustomAttribute<BrowsableAttribute>()?.Browsable ?? true))
                .Select(x => x.GetValue(enumType))
                .ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}