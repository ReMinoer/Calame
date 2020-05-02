using System.ComponentModel.Composition;
using System.Windows.Media;
using Fingear.Interactives;
using Fingear.Interactives.Interfaces;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    public class InteractiveIconDescriptor : IDefaultIconDescriptorModule<IInteractive>
    {
        static public readonly Brush CoreCategoryBrush = Brushes.DimGray;
        static public readonly Brush UiCategoryBrush = Brushes.OrangeRed;

        public IconDescription GetDefaultIcon(IInteractive interactive)
        {
            switch (interactive)
            {
                case IInteractiveInterface _:
                    return new IconDescription(PackIconMaterialKind.ViewDashboard, UiCategoryBrush);
                default:
                    return new IconDescription(PackIconMaterialKind.Layers, CoreCategoryBrush);
            }
        }

        public IconDescription GetIcon(IInteractive interactive)
        {
            switch (interactive)
            {
                case InteractiveInterfaceRoot _:
                    return new IconDescription(PackIconMaterialKind.ViewDashboardVariant, UiCategoryBrush);
                default:
                    return IconDescription.None;
            }
        }
    }
}