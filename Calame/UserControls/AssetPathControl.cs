using System.IO;
using Calame.Dialogs;

namespace Calame.UserControls
{
    public class AssetPathControl : PathControl
    {
        protected override bool? ValidatePath(ref string path)
        {
            bool? validatePath = base.ValidatePath(ref path);
            if (validatePath != true)
                return validatePath;

            if (HasValidRoot(path))
            {
                path = Path.ChangeExtension(path, null);
                return true;
            }

            //assetPath = ImportAssetDialog.ShowDialog(path, RootFolder, IconProvider, IconDescriptorManager);

            var importAssetDialog = new ImportAssetDialog
            {
                TargetedFilePath = path,
                AssetName = Path.GetFileNameWithoutExtension(path),
                ImportFolderPath = RootFolder,
                ContentRootPath = RootFolder,
                IconProvider = IconProvider,
                IconDescriptorManager = IconDescriptorManager
            };

            importAssetDialog.DataContext = importAssetDialog;
            if (importAssetDialog.ShowDialog() != true)
                return null;

            path = importAssetDialog.ImportPath;
            return true;
        }
    }
}