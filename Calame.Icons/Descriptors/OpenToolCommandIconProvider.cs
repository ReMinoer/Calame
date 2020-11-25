using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using Gemini.Framework.Commands;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<CommandDefinition>))]
    public class OpenToolCommandIconProvider : IconDescriptorModuleBase<CommandDefinition>
    {
        static public readonly Brush DefaultBrush = Brushes.DimGray;

        public override IconDescription GetIcon(CommandDefinition key)
        {
            switch (key.Name)
            {
                case "BrushPanel.Open":
                    return new IconDescription(PackIconMaterialKind.Palette, DefaultBrush);
                case "CompositionGraph.Open":
                    return new IconDescription(PackIconMaterialKind.HexagonMultiple, DefaultBrush);
                case "DataModelTree.Open":
                    return new IconDescription(PackIconMaterialKind.HexagonMultipleOutline, DefaultBrush);
                case "InteractionTree.Open":
                    return new IconDescription(PackIconMaterialKind.GestureTap, DefaultBrush);
                case "LogConsole.Open":
                    return new IconDescription(PackIconMaterialKind.Console, DefaultBrush);
                case "PropertyGrid.Open":
                    return new IconDescription(PackIconMaterialKind.FormatListBulletedType, DefaultBrush);
                case "SceneGraph.Open":
                    return new IconDescription(PackIconMaterialKind.AxisArrow, DefaultBrush);
                default:
                    return IconDescription.None;
            }
        }
    }
}