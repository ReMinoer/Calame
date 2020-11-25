using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using Gemini.Framework.Commands;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<CommandDefinition>))]
    [Export(typeof(ITypeIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule<CommandDefinition>))]
    public class OpenToolCommandIconDescriptor : TypeIconDescriptorModuleBase<CommandDefinition>
    {
        static public readonly Brush DefaultBrush = Brushes.DimGray;

        public override IconDescription GetTypeIcon(Type type)
        {
            switch (type.Name)
            {
                case "BrushPanelCommand":
                    return new IconDescription(PackIconMaterialKind.Palette, DefaultBrush);
                case "CompositionGraphCommand":
                    return new IconDescription(PackIconMaterialKind.HexagonMultiple, DefaultBrush);
                case "DataModelTreeCommand":
                    return new IconDescription(PackIconMaterialKind.HexagonMultipleOutline, DefaultBrush);
                case "InteractionTreeCommand":
                    return new IconDescription(PackIconMaterialKind.GestureTap, DefaultBrush);
                case "LogConsoleCommand":
                    return new IconDescription(PackIconMaterialKind.Console, DefaultBrush);
                case "PropertyGridCommand":
                    return new IconDescription(PackIconMaterialKind.FormatListBulletedType, DefaultBrush);
                case "SceneGraphCommand":
                    return new IconDescription(PackIconMaterialKind.AxisArrow, DefaultBrush);
                default:
                    return IconDescription.None;
            }
        }
    }
}