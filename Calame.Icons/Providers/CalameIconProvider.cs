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
                case CalameIconKey.ViewGraph: return PackIconMaterialKind.Monitor;

                case CalameIconKey.Play: return PackIconMaterialKind.Play;
                case CalameIconKey.Pause: return PackIconMaterialKind.Pause;
                case CalameIconKey.Stop: return PackIconMaterialKind.Stop;
                case CalameIconKey.NextFrame: return PackIconMaterialKind.SkipNext;
                case CalameIconKey.Reset: return PackIconMaterialKind.RotateLeft;
                case CalameIconKey.ViewerDebugMode: return PackIconMaterialKind.ApplicationCog;

                case CalameIconKey.DefaultCamera: return PackIconMaterialKind.CastVariant;
                case CalameIconKey.FreeCamera: return PackIconMaterialKind.Video;
                case CalameIconKey.ResetCamera: return PackIconMaterialKind.CameraMeteringMatrix;
                case CalameIconKey.FocusCamera: return PackIconMaterialKind.CameraMeteringCenter;
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
                case CalameIconKey.Select: return PackIconMaterialKind.ArrowTopRightThick;
                case CalameIconKey.OpenWith: return PackIconMaterialKind.ApplicationImport;
                case CalameIconKey.EditableItem: return PackIconMaterialKind.Pencil;
                case CalameIconKey.EditableProperties: return PackIconMaterialKind.PlaylistEdit;
                case CalameIconKey.ReadOnlyProperties: return PackIconMaterialKind.Eye;

                case CalameIconKey.CollapseAll: return PackIconMaterialKind.CollapseAllOutline;
                case CalameIconKey.ExpandAll: return PackIconMaterialKind.ExpandAllOutline;
                case CalameIconKey.ShowSelection: return PackIconMaterialKind.CrosshairsGps;

                case CalameIconKey.Clear: return PackIconMaterialKind.PlaylistRemove;
                case CalameIconKey.ScrollToEnd: return PackIconMaterialKind.ArchiveArrowDownOutline;
                case CalameIconKey.AutoScroll: return PackIconMaterialKind.AutoDownload;

                default: return default(PackIconMaterialKind);
            }
        }
    }
}