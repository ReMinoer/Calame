using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons;
using Calame.Icons.Base;
using Calame.SceneViewer;
using Diese;
using MahApps.Metro.IconPacks;

namespace Calame.Demo.Modules.DemoGameData
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<ISession>))]
    [Export(typeof(ITypeIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule<ISession>))]
    public class SessionIconDescriptor : TypeIconDescriptorModuleBase<ISession>
    {
        static public readonly Brush DefaultBrush = Brushes.DimGray;

        public override IconDescription GetTypeIcon(Type type)
        {
            if (type.Is<MovingSession>())
                return new IconDescription(PackIconMaterialKind.Gamepad, DefaultBrush);

            return IconDescription.None;
        }
    }
}