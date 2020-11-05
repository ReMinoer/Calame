using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using Diese;
using Fingear.Interactives;
using Fingear.Interactives.Interfaces;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<IInteractive>))]
    [Export(typeof(IDefaultIconDescriptorModule<IInteractive>))]
    [Export(typeof(ITypeIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule<IInteractive>))]
    [Export(typeof(ITypeDefaultIconDescriptorModule<IInteractive>))]
    public class InteractiveIconDescriptor : TypeHybridIconDescriptorModuleBase<IInteractive>
    {
        static public readonly Brush CoreCategoryBrush = Brushes.DimGray;
        static public readonly Brush UiCategoryBrush = Brushes.OrangeRed;

        public override IconDescription GetTypeDefaultIcon(Type type)
        {
            if (type.Is<IInteractiveInterface>())
                return new IconDescription(PackIconMaterialKind.ViewDashboard, UiCategoryBrush);
            if (type.Is<IInteractive>())
                return new IconDescription(PackIconMaterialKind.Layers, CoreCategoryBrush);

            return IconDescription.None;
        }

        public override IconDescription GetTypeIcon(Type type)
        {
            if (type.Is<InteractiveInterfaceRoot>())
                return new IconDescription(PackIconMaterialKind.ViewDashboardVariant, UiCategoryBrush);

            return IconDescription.None;
        }
    }
}