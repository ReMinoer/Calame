using System;
using System.Globalization;
using System.Windows.Data;
using Glyph.IO;

namespace Calame.Converters
{
    public class AssetPathToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;

            return (string)(AssetPath)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;

            return (AssetPath)(string)value;
        }
    }
}