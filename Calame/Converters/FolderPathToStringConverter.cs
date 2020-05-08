using System;
using System.Globalization;
using System.Windows.Data;
using Glyph.IO;

namespace Calame.Converters
{
    public class FolderPathToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)(FolderPath)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FolderPath)(string)value;
        }
    }
}