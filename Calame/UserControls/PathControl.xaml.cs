using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calame.Icons;
using Glyph.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Calame.UserControls
{
    public partial class PathControl : UserControl, INotifyPropertyChanged
    {
        static readonly char AbsoluteSeparator = Path.DirectorySeparatorChar;
        static readonly char RelativeSeparator = Path.AltDirectorySeparatorChar;

        static public readonly DependencyProperty UserPathProperty =
            DependencyProperty.Register(nameof(UserPath), typeof(string), typeof(PathControl), new PropertyMetadata(null, OnUserPathChanged));

        static public readonly DependencyProperty RootFolderProperty =
            DependencyProperty.Register(nameof(RootFolder), typeof(string), typeof(PathControl), new PropertyMetadata(null, OnRootChanged), ValidateRoot);
        static public readonly DependencyProperty RelativePathOnlyProperty =
            DependencyProperty.Register(nameof(RelativePathOnly), typeof(bool), typeof(PathControl), new PropertyMetadata(false));

        static public readonly DependencyProperty FolderModeProperty =
            DependencyProperty.Register(nameof(FolderMode), typeof(bool), typeof(PathControl), new PropertyMetadata(false));
        static public readonly DependencyProperty ShowOpenPathButtonProperty =
            DependencyProperty.Register(nameof(ShowOpenPathButton), typeof(bool), typeof(PathControl), new PropertyMetadata(false));
        static public readonly DependencyProperty OpenPathCommandProperty =
            DependencyProperty.Register(nameof(OpenPathCommand), typeof(ICommand), typeof(PathControl), new PropertyMetadata(null));
        static public readonly DependencyProperty FileTypesProperty =
            DependencyProperty.Register(nameof(FileTypes), typeof(IList<FileType>), typeof(PathControl), new PropertyMetadata(null));

        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(PathControl), new PropertyMetadata(null));
        static public readonly DependencyProperty IconDescriptorManagerProperty =
            DependencyProperty.Register(nameof(IconDescriptorManager), typeof(IIconDescriptorManager), typeof(PathControl), new PropertyMetadata(null, OnIconDescriptorManagerChanged));

        public string UserPath
        {
            get => (string)GetValue(UserPathProperty);
            set => SetValue(UserPathProperty, value);
        }

        static private void OnUserPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathControl pathControl = (PathControl)d;
            string newValue = (string)e.NewValue;

            pathControl.SetDisplayedPath(pathControl.ConvertToRelativePathIfPossible(newValue));
            pathControl.OnPropertyChanged(nameof(FullPath));
            pathControl.OnPropertyChanged(nameof(ShowOpenPathButton));
        }

        private string _displayedPath;
        public string DisplayedPath
        {
            get => _displayedPath;
            set
            {
                if (value == string.Empty)
                    value = null;
                SetDisplayedPath(value);
            }
        }

        private void SetDisplayedPath(string value)
        {
            if (!Set(ref _displayedPath, value, nameof(DisplayedPath)))
                return;

            UserPath = ConvertToRelativePathIfPossible(_displayedPath);
        }

        public string FullPath => ConvertToFullPath(UserPath);

        private string _normalizedRoot;
        public string RootFolder
        {
            get => (string)GetValue(RootFolderProperty);
            set => SetValue(RootFolderProperty, value);
        }

        static private bool ValidateRoot(object value) => value == null || Path.IsPathRooted((string)value);

        static private void OnRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathControl pathControl = (PathControl)d;
            string newValue = (string)e.NewValue;

            string fullPath = pathControl.FullPath;

            pathControl._normalizedRoot = newValue != null ? pathControl.NormalizePath(newValue, asDirectory: true) : null;

            pathControl.SetDisplayedPath(pathControl.ConvertToRelativePathIfPossible(fullPath));
            pathControl.OnPropertyChanged(nameof(FullPath));
            pathControl.OnPropertyChanged(nameof(ShowOpenPathButton));
        }

        public bool RelativePathOnly
        {
            get => (bool)GetValue(RelativePathOnlyProperty);
            set => SetValue(RelativePathOnlyProperty, value);
        }

        public bool FolderMode
        {
            get => (bool)GetValue(FolderModeProperty);
            set => SetValue(FolderModeProperty, value);
        }

        public bool ShowOpenPathButton
        {
            get => (bool)GetValue(ShowOpenPathButtonProperty);
            set => SetValue(ShowOpenPathButtonProperty, value);
        }

        public ICommand OpenPathCommand
        {
            get => (ICommand)GetValue(OpenPathCommandProperty);
            set => SetValue(OpenPathCommandProperty, value);
        }

        public IList<FileType> FileTypes
        {
            get => (IList<FileType>)GetValue(FileTypesProperty);
            set => SetValue(FileTypesProperty, value);
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

        private IIconDescriptor _systemIconDescriptor;
        public IIconDescriptor SystemIconDescriptor
        {
            get => _systemIconDescriptor;
            private set => Set(ref _systemIconDescriptor, value);
        }

        private IIconDescriptor _fileTypeIconDescriptor;
        public IIconDescriptor FileTypeIconDescriptor
        {
            get => _fileTypeIconDescriptor;
            private set => Set(ref _fileTypeIconDescriptor, value);
        }

        public CalameIconKey DefaultIconKey => FolderMode ? CalameIconKey.Folder : CalameIconKey.File;

        public PathControl()
        {
            InitializeComponent();
        }

        private void OnBrowseButtonClicked(object sender, RoutedEventArgs e)
        {
            string path = UserPath;
            while (true)
            {
                path = FolderMode ? AskFolderPath(lastPath: path) : AskFilePath(lastPath: path);
                if (path == null)
                    return;

                path = NormalizePath(path);
                bool? valid = ValidatePath(ref path);
                if (valid == true)
                    break;
                if (valid == null)
                    return;
            }

            SetDisplayedPath(ConvertToRelativePathIfPossible(path));
        }

        protected virtual bool? ValidatePath(ref string path)
        {
            if (RelativePathOnly && _normalizedRoot != null && !path.StartsWith(_normalizedRoot))
            {
                MessageBox.Show($"Selected path is not part of {RootFolder}!", "Invalid path", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private string AskFilePath(string lastPath)
        {
            var dialog = new OpenFileDialog
            {
                Filter = FileTypes != null ? string.Join("|", FileTypes.Select(x => $"{x.DisplayName}|{string.Join(";", x.Extensions.Select(ext => "*" + ext))}")) : string.Empty,
                DefaultExt = FileTypes?.FirstOrDefault().Extensions?.First() ?? string.Empty
            };

            if (lastPath != null)
            {
                string systemPath = ConvertToFullPath(lastPath);
                if (File.Exists(systemPath))
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(_normalizedRoot) ?? _normalizedRoot;
                    dialog.FileName = Path.GetFileName(systemPath);
                }
            }
            else
            {
                dialog.InitialDirectory = _normalizedRoot;
            }

            if (dialog.ShowDialog(Window.GetWindow(this)) == true)
                return dialog.FileName;

            return null;
        }

        private string AskFolderPath(string lastPath)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (lastPath != null)
            {
                string systemPath = ConvertToFullPath(lastPath);
                if (Directory.Exists(systemPath))
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(_normalizedRoot) ?? _normalizedRoot;
                    dialog.DefaultFileName = Path.GetFileName(systemPath);
                }
            }
            else
            {
                dialog.InitialDirectory = _normalizedRoot;
            }

            if (dialog.ShowDialog(Window.GetWindow(this)) == CommonFileDialogResult.Ok)
                return dialog.FileName;

            return null;
        }

        private string ConvertToRelativePathIfPossible(string path, bool? asDirectory = null)
        {
            if (path == null)
                return null;

            path = NormalizePath(path, asDirectory);

            if (HasValidRoot(path))
                return path.Substring(_normalizedRoot.Length);
            
            return path;
        }

        private string ConvertToFullPath(string path, bool? asDirectory = null)
        {
            if (path == null)
                return null;

            path = NormalizePath(path, asDirectory);

            if (_normalizedRoot != null && !Path.IsPathRooted(path))
                return NormalizePath(Path.Combine(_normalizedRoot, path), asDirectory);

            return path;
        }

        protected bool HasValidRoot(string path) => _normalizedRoot != null && Path.IsPathRooted(path) && path.StartsWith(_normalizedRoot);

        private string NormalizePath(string path, bool? asDirectory = null)
        {
            // Use unique separator
            bool isAbsolute = Path.IsPathRooted(path);
            char separator = isAbsolute ? AbsoluteSeparator : RelativeSeparator;
            char otherSeparator = isAbsolute ? RelativeSeparator : AbsoluteSeparator;
            path = path.Replace(otherSeparator, separator);

            // Add end separator if directory
            if ((asDirectory ?? FolderMode) && path.Length > 0 && path[path.Length - 1] != separator)
                path += separator;

            return path;
        }

        static private void OnIconDescriptorManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (PathControl)d;

            propertyGrid.SystemIconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor<CalameIconKey>();
            propertyGrid.FileTypeIconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor<FileType>();
            propertyGrid.OnPropertyChanged(nameof(DefaultIconKey));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
