using System.ComponentModel.Composition;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Providers
{
    [Export(typeof(IIconProviderModule))]
    public class MaterialIconProvider : IIconProviderModule
    {
        public bool Handle(IconDescription iconDescription)
        {
            return iconDescription.Key is PackIconMaterialKind;
        }

        public Control GetControl(IconDescription iconDescription, int size)
        {
            return new PackIconMaterial
            {
                Kind = (PackIconMaterialKind)iconDescription.Key,
                Foreground = iconDescription.Brush,
                Width = size,
                Height = size,
                Focusable = false
            };
        }
    }
}