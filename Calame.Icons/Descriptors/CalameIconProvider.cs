using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<CalameIconKey>))]
    public class CalameIconProvider : IconDescriptorModuleBase<CalameIconKey>
    {
        static public readonly Brush ToolsBrush = Brushes.DimGray;
        static public readonly Brush DefaultBrush = Brushes.Black;

        public override IconDescription GetIcon(CalameIconKey key)
        {
            switch (key)
            {
                case CalameIconKey.BrushPanel: return new IconDescription(PackIconMaterialKind.Palette, ToolsBrush);
                case CalameIconKey.CompositionGraph: return new IconDescription(PackIconMaterialKind.HexagonMultiple, ToolsBrush);
                case CalameIconKey.DataModelTree: return new IconDescription(PackIconMaterialKind.HexagonMultipleOutline, ToolsBrush);
                case CalameIconKey.InteractionTree: return new IconDescription(PackIconMaterialKind.GestureTap, ToolsBrush);
                case CalameIconKey.LogConsole: return new IconDescription(PackIconMaterialKind.Console, ToolsBrush);
                case CalameIconKey.PropertyGrid: return new IconDescription(PackIconMaterialKind.FormatListBulletedType, ToolsBrush);
                case CalameIconKey.SceneGraph: return new IconDescription(PackIconMaterialKind.AxisArrow, ToolsBrush);

                case CalameIconKey.Play: return new IconDescription(PackIconMaterialKind.Play, Brushes.Green);
                case CalameIconKey.Pause: return new IconDescription(PackIconMaterialKind.Pause, Brushes.CornflowerBlue);
                case CalameIconKey.Stop: return new IconDescription(PackIconMaterialKind.Stop, Brushes.DarkRed);

                case CalameIconKey.DefaultCamera: return new IconDescription(PackIconMaterialKind.AppleAirplay, DefaultBrush);
                case CalameIconKey.FreeCamera: return new IconDescription(PackIconMaterialKind.Video, DefaultBrush);
                case CalameIconKey.NewViewer: return new IconDescription(PackIconMaterialKind.ShapeRectanglePlus, DefaultBrush);

                case CalameIconKey.GameMode: return new IconDescription(PackIconMaterialKind.GamepadVariant, DefaultBrush);
                case CalameIconKey.CursorMode: return new IconDescription(PackIconMaterialKind.CursorDefaultOutline, DefaultBrush);
                case CalameIconKey.BrushMode: return new IconDescription(PackIconMaterialKind.Brush, DefaultBrush);

                case CalameIconKey.Previous: return new IconDescription(PackIconMaterialKind.ArrowLeftCircle, DefaultBrush);
                case CalameIconKey.Next: return new IconDescription(PackIconMaterialKind.ArrowRightCircle, DefaultBrush);
                case CalameIconKey.File: return new IconDescription(PackIconMaterialKind.FileOutline, DefaultBrush);
                case CalameIconKey.Folder: return new IconDescription(PackIconMaterialKind.FolderOpen, DefaultBrush);
                case CalameIconKey.Add: return new IconDescription(PackIconMaterialKind.PlusCircle, DefaultBrush);
                case CalameIconKey.AddFromList: return new IconDescription(PackIconMaterialKind.PlaylistPlus, DefaultBrush);
                case CalameIconKey.Delete: return new IconDescription(PackIconMaterialKind.CloseCircle, DefaultBrush);
                case CalameIconKey.Select: return new IconDescription(PackIconMaterialKind.ArrowTopRight, DefaultBrush);
                case CalameIconKey.OpenWith: return new IconDescription(PackIconMaterialKind.ApplicationImport, DefaultBrush);
                case CalameIconKey.ReadOnlyProperties: return new IconDescription(PackIconMaterialKind.Eye, DefaultBrush);
                case CalameIconKey.EditableProperties: return new IconDescription(PackIconMaterialKind.PlaylistEdit, DefaultBrush);

                default: return IconDescription.None; 
            }
        }
    }
}