using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using Fingear.Interactives;
using Fingear.Interactives.Interfaces;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<IInteractive>))]
    [Export(typeof(IDefaultIconDescriptorModule<IInteractive>))]
    public class InteractiveIconDescriptor : HybridIconDescriptorModuleBase<IInteractive>
    {
        static public readonly Brush CoreCategoryBrush = Brushes.DimGray;
        static public readonly Brush UiCategoryBrush = Brushes.OrangeRed;

        public override IconDescription GetDefaultIcon(IInteractive interactive)
        {
            switch (interactive)
            {
                case IInteractiveInterface _:
                    return new IconDescription(PackIconMaterialKind.ViewDashboard, UiCategoryBrush);
                default:
                    return new IconDescription(PackIconMaterialKind.Layers, CoreCategoryBrush);
            }
        }

        public override IconDescription GetIcon(IInteractive interactive)
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