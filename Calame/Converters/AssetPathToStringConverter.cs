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
            return (string)(AssetPath)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AssetPath)(string)value;
        }
    }
}