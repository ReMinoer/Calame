using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Glyph.Core;

namespace Calame.SceneGraph.Converters
{
    public class GlyphObjectFilter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as IEnumerable<SceneNode>;
            if (enumerable == null)
                return Enumerable.Empty<SceneNode>();

            return enumerable.Where(x => x.Parent == null || x.Parent is GlyphObject);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}