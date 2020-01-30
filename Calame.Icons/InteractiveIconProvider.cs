using System.ComponentModel.Composition;
using System.Windows.Media;
using Fingear;
using MahApps.Metro.IconPacks;

namespace Calame.Icons
{
    [Export(typeof(IIconDescriptorModule))]
    public class InteractiveIconProvider : IDefaultIconDescriptorModule<IInteractive>
    {
        static public readonly Brush CoreCategoryBrush = Brushes.DimGray;

        public IconDescription GetDefaultIcon(IInteractive interactive)
        {
            return new IconDescription(PackIconMaterialKind.Layers, CoreCategoryBrush);
        }

        public IconDescription GetIcon(IInteractive interactive)
        {
            return IconDescription.None;
        }
    }
}