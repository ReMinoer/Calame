using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Glyph.IO;

namespace Calame.Converters
{
    public class AttributesToFileTypesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var attributes = (AttributeCollection)value;
            return attributes?.OfType<FileTypeAttribute>().Select(x => x.FileType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}