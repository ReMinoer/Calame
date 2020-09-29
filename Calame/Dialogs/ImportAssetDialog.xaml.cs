using System;
using System.IO;
using System.Windows;
using Calame.Icons;
using Glyph;

namespace Calame.Dialogs
{
    public partial class ImportAssetDialog : Window
    {
        static public readonly DependencyProperty TargetedFilePathProperty =
            DependencyProperty.Register(nameof(TargetedFilePath), typeof(string), typeof(ImportAssetDialog), new PropertyMetadata(null));
        static public readonly DependencyProperty AssetNameProperty =
            DependencyProperty.Register(nameof(AssetName), typeof(string), typeof(ImportAssetDialog), new PropertyMetadata(null));
        static public readonly DependencyProperty ImportFolderPathProperty =
            DependencyProperty.Register(nameof(ImportFolderPath), typeof(string), typeof(ImportAssetDialog), new PropertyMetadata(null));
        static public readonly DependencyProperty ContentRootPathProperty =
            DependencyProperty.Register(nameof(ContentRootPath), typeof(string), typeof(ImportAssetDialog), new PropertyMetadata(null));
        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(ImportAssetDialog), new PropertyMetadata(null));
        static public readonly DependencyProperty IconDescriptorManagerProperty =
            DependencyProperty.Register(nameof(IconDescriptorManager), typeof(IIconDescriptorManager), typeof(ImportAssetDialog), new PropertyMetadata(null));

        public string TargetedFilePath
        {
            get => (string)GetValue(TargetedFilePathProperty);
            set => SetValue(TargetedFilePathProperty, value);
        }

        public string AssetName
        {
            get => (string)GetValue(AssetNameProperty);
            set => SetValue(AssetNameProperty, value);
        }

        public string ImportFolderPath
        {
            get => (string)GetValue(ImportFolderPathProperty);
            set => SetValue(ImportFolderPathProperty, value);
        }

        public string ContentRootPath
        {
            get => (string)GetValue(ContentRootPathProperty);
            set => SetValue(ContentRootPathProperty, value);
        }

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        public IIconDescriptorManager IconDescriptorManager
        {
            get => (IIconDescriptorManager)GetValue(IconDescriptorManagerProperty);
            set => SetValue(IconDescriptorManagerProperty, value);
        }

        public string ImportPath => ImportFolderPath != null && AssetName != null
            ? Path.Combine(ImportFolderPath, AssetName)
            : null;

        public ImportAssetDialog()
        {
            InitializeComponent();
        }

        static public string ShowDialog(string targetedFilePath, IContentLibrary contentLibrary, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
        {
            return ShowDialog(targetedFilePath, contentLibrary.WorkingDirectory, iconProvider, iconDescriptorManager);
        }

        static public string ShowDialog(string targetedFilePath, string contentRootPath, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
        {
            var importAssetDialog = new ImportAssetDialog
            {
                TargetedFilePath = targetedFilePath,
                AssetName = Path.GetFileNameWithoutExtension(targetedFilePath),
                ImportFolderPath = contentRootPath,
                ContentRootPath = contentRootPath,
                IconProvider = iconProvider,
                IconDescriptorManager = iconDescriptorManager
            };

            importAssetDialog.DataContext = importAssetDialog;
            return importAssetDialog.ShowDialog() == true ? importAssetDialog.ImportPath : null;
        }

        private void OnImport(object sender, RoutedEventArgs e)
        {
            if (ImportPath == null)
                return;

            string importFolderFullPath = Path.Combine(ContentRootPath, ImportFolderPath);
            Directory.CreateDirectory(importFolderFullPath);

            string fileName = AssetName + Path.GetExtension(TargetedFilePath);
            string importFullPath = Path.Combine(importFolderFullPath, fileName);
            if (File.Exists(importFullPath))
            {
                string message = $"Asset \"{Path.Combine(ImportFolderPath, fileName)}\" already exists. Are you sure you want to overwrite it ?";

                MessageBoxResult messageBoxResult = MessageBox.Show(message, "Asset already exists", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (messageBoxResult == MessageBoxResult.Cancel)
                    return;
            }

            File.Copy(TargetedFilePath, importFullPath, overwrite: true);
            DialogResult = true;
        }
    }
}
