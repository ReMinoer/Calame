using System;
using System.Collections.Generic;
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

            var fileTypes = new List<FileType>();
            if (attributes != null)
                fileTypes.AddRange(attributes.OfType<FileTypeAttribute>().Select(x => x.FileType));
            if (parameter != null)
                fileTypes.AddRange(parameter.ToString().Split('|').Select(x => new FileType(x)));

            return fileTypes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}