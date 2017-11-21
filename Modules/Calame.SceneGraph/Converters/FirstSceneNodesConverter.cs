using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;

namespace Calame.SceneGraph.Converters
{
    public class FirstSceneNodesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var component = value as IGlyphComponent;
            if (component == null)
                return Enumerable.Empty<SceneNode>();

            return Convert(component);
        }

        private IEnumerable<object> Convert(IGlyphComponent glyphComponent)
        {
            if (glyphComponent.Components.Any(out SceneNode sceneNode))
            {
                yield return sceneNode;
                yield break;
            }

            foreach (IGlyphComponent component in glyphComponent.Components)
                foreach (object obj in Convert(component))
                    yield return obj;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}