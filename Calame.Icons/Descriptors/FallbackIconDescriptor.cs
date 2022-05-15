using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IFallbackIconDescriptorModule))]
    public class FallbackIconDescriptor : IFallbackIconDescriptorModule
    {
        static public readonly Brush DefaultBrush = IconBrushes.Default;

        public bool Handle(object model) => true;
        public bool Handle(Type type) => true;

        public IconDescription GetBaseTypeIcon(Type type) => GetIcon(type);
        public IconDescription GetBaseTypeIcon(object model) => GetIcon(model);
        private IconDescription GetIcon(object model)
        {
            if (model == null)
                return new IconDescription(PackIconMaterialKind.Null, DefaultBrush);
            if (model is IEnumerable)
                return new IconDescription(PackIconMaterialKind.Menu, DefaultBrush);

            return new IconDescription(PackIconMaterialKind.RhombusOutline, DefaultBrush);
        }
    }
}