using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Diese.Collections;
using Glyph.IO;

namespace Calame.Converters
{
    public class AttributesToRootFolderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var attributes = (AttributeCollection)value;
            return attributes?.FirstOfTypeOrDefault<RootFolderAttribute>().Path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}