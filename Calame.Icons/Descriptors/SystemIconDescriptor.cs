using System.ComponentModel.Composition;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    public class SystemIconProvider : IDefaultIconDescriptorModule<SystemIconKey>
    {
        public IconDescription GetIcon(SystemIconKey key)
        {
            switch (key)
            {
                case SystemIconKey.File: return new IconDescription(PackIconMaterialKind.FileOutline, Brushes.Black);
                case SystemIconKey.Folder: return new IconDescription(PackIconMaterialKind.FolderOpen, Brushes.Black);
                case SystemIconKey.Add: return new IconDescription(PackIconMaterialKind.PlusCircle, Brushes.Black);
                case SystemIconKey.AddFromList: return new IconDescription(PackIconMaterialKind.PlaylistPlus, Brushes.Black);
                case SystemIconKey.Delete: return new IconDescription(PackIconMaterialKind.CloseCircle, Brushes.Black);
                case SystemIconKey.ShowIn: return new IconDescription(PackIconMaterialKind.ArrowTopRight, Brushes.Black);
                default: return IconDescription.None;
            }
        }

        public IconDescription GetDefaultIcon(SystemIconKey model) => IconDescription.None;
    }
}