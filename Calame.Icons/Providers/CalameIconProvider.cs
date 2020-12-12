using System.ComponentModel.Composition;
using Calame.Icons.Providers.Base;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Providers
{
    [Export(typeof(IIconProviderModule))]
    [Export(typeof(IIconProviderModule<CalameIconKey>))]
    public class CalameIconProvider : ReTargetingIconProviderBase<CalameIconKey, PackIconMaterialKind>
    {
        protected override PackIconMaterialKind GetTargetKey(CalameIconKey key)
        {
            switch (key)
            {
                case CalameIconKey.BrushPanel: return PackIconMaterialKind.Palette;
                case CalameIconKey.CompositionGraph: return PackIconMaterialKind.HexagonMultiple;
                case CalameIconKey.DataModelTree: return PackIconMaterialKind.HexagonMultipleOutline;
                case CalameIconKey.InteractionTree: return PackIconMaterialKind.GestureTap;
                case CalameIconKey.LogConsole: return PackIconMaterialKind.Console;
                case CalameIconKey.PropertyGrid: return PackIconMaterialKind.FormatListBulletedType;
                case CalameIconKey.SceneGraph: return PackIconMaterialKind.AxisArrow;

                case CalameIconKey.Play: return PackIconMaterialKind.Play;
                case CalameIconKey.Pause: return PackIconMaterialKind.Pause;
                case CalameIconKey.Stop: return PackIconMaterialKind.Stop;

                case CalameIconKey.DefaultCamera: return PackIconMaterialKind.AppleAirplay;
                case CalameIconKey.FreeCamera: return PackIconMaterialKind.Video;
                case CalameIconKey.NewViewer: return PackIconMaterialKind.ShapeRectanglePlus;

                case CalameIconKey.SessionMode: return PackIconMaterialKind.GamepadVariant;
                case CalameIconKey.EditorMode: return PackIconMaterialKind.CursorDefaultOutline;
                case CalameIconKey.BrushMode: return PackIconMaterialKind.Brush;

                case CalameIconKey.Previous: return PackIconMaterialKind.ArrowLeftCircle;
                case CalameIconKey.Next: return PackIconMaterialKind.ArrowRightCircle;
                case CalameIconKey.File: return PackIconMaterialKind.FileOutline;
                case CalameIconKey.Folder: return PackIconMaterialKind.FolderOpen;
                case CalameIconKey.Add: return PackIconMaterialKind.PlusCircle;
                case CalameIconKey.AddFromList: return PackIconMaterialKind.PlaylistPlus;
                case CalameIconKey.Delete: return PackIconMaterialKind.CloseCircle;
                case CalameIconKey.Select: return PackIconMaterialKind.ArrowTopRight;
                case CalameIconKey.OpenWith: return PackIconMaterialKind.ApplicationImport;
                case CalameIconKey.EditableItem: return PackIconMaterialKind.Pencil;
                case CalameIconKey.EditableProperties: return PackIconMaterialKind.PlaylistEdit;
                case CalameIconKey.ReadOnlyProperties: return PackIconMaterialKind.Eye;

                default: return default(PackIconMaterialKind);
            }
        }
    }
}