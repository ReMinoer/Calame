using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Calame.ContentFileTypes;
using Glyph.IO;

namespace Calame.Converters
{
    public class AttributesToFileTypesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var attributes = values[0] as AttributeCollection;
            var fileExtensionResolver = (IContentFileTypeResolver)values[1];

            var fileTypes = new List<FileType>();

            if (attributes != null)
            {
                fileTypes.AddRange(attributes.OfType<FileTypeAttribute>().Select(x => x.FileType));

                if (fileExtensionResolver != null)
                    fileTypes.AddRange(attributes.OfType<FileContentAttribute>().Select(x => x.ContentType).SelectMany(fileExtensionResolver.GetFileTypes));
            }

            if (parameter != null)
                fileTypes.AddRange(parameter.ToString().Split('|').Select(x => new FileType(x)));

            string[] allExtensions = fileTypes.SelectMany(x => x.Extensions).Distinct().ToArray();
            if (allExtensions.Length > 1 && allExtensions.Length <= 10)
            {
                fileTypes.Insert(0, new FileType
                {
                    DisplayName = "Supported Files",
                    Extensions = allExtensions
                });
            }
            else if (allExtensions.Length > 10)
            {
                fileTypes.Insert(0, FileType.All);
            }

            return fileTypes;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new []{Binding.DoNothing, Binding.DoNothing};
        }
    }
}