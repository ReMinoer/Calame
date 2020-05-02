using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Calame.UserControls
{
    public partial class PathControl : UserControl, INotifyPropertyChanged
    {
        static readonly char AbsoluteSeparator = Path.DirectorySeparatorChar;
        static readonly char RelativeSeparator = Path.AltDirectorySeparatorChar;

        static public readonly DependencyProperty UserPathProperty =
            DependencyProperty.Register(nameof(UserPath), typeof(string), typeof(PathControl), new PropertyMetadata(null, OnUserPathChanged));
        static public readonly DependencyProperty RootProperty =
            DependencyProperty.Register(nameof(Root), typeof(string), typeof(PathControl), new PropertyMetadata(null, OnRootChanged), ValidateRoot);
        static public readonly DependencyProperty DirectoryModeProperty =
            DependencyProperty.Register(nameof(DirectoryMode), typeof(bool), typeof(PathControl), new PropertyMetadata(false));

        public string UserPath
        {
            get => (string)GetValue(UserPathProperty);
            set => SetValue(UserPathProperty, value);
        }

        static private void OnUserPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathControl pathControl = (PathControl)d;
            string newValue = (string)e.NewValue;

            pathControl.DisplayedPath = pathControl.ConvertToRelativePathIfPossible(newValue);
        }

        private string _displayedPath;
        public string DisplayedPath
        {
            get => _displayedPath;
            set
            {
                if (!Set(ref _displayedPath, value))
                    return;

                UserPath = ConvertToRelativePathIfPossible(_displayedPath);
            }
        }

        private string _normalizedRoot;
        public string Root
        {
            get => (string)GetValue(RootProperty);
            set => SetValue(RootProperty, value);
        }

        static private bool ValidateRoot(object value) => value == null || Path.IsPathRooted((string)value);

        static private void OnRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathControl pathControl = (PathControl)d;
            string newValue = (string)e.NewValue;

            pathControl._normalizedRoot = newValue != null ? pathControl.NormalizePath(newValue, asDirectory: true) : null;
        }

        public bool DirectoryMode
        {
            get => (bool)GetValue(DirectoryModeProperty);
            set => SetValue(DirectoryModeProperty, value);
        }

        public PathControl()
        {
            InitializeComponent();
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            if (DirectoryMode)
            {
                throw new NotSupportedException();
            }
            else
            {
                var dialog = new OpenFileDialog
                {
                    InitialDirectory = _normalizedRoot
                };

                if (UserPath != null)
                {
                    string systemPath = ConvertToFullPath(UserPath);
                    if (File.Exists(systemPath))
                        dialog.FileName = systemPath;
                }

                if (dialog.ShowDialog() == true)
                    DisplayedPath = ConvertToRelativePathIfPossible(dialog.FileName);
            }
        }

        private string ConvertToRelativePathIfPossible(string path, bool? asDirectory = null)
        {
            if (path == null)
                return null;

            path = NormalizePath(path, asDirectory);

            if (_normalizedRoot != null && Path.IsPathRooted(path) && path.StartsWith(_normalizedRoot))
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

        private string NormalizePath(string path, bool? asDirectory = null)
        {
            // Use unique separator
            bool isAbsolute = Path.IsPathRooted(path);
            char separator = isAbsolute ? AbsoluteSeparator : RelativeSeparator;
            char otherSeparator = isAbsolute ? RelativeSeparator : AbsoluteSeparator;
            path = path.Replace(otherSeparator, separator);

            // Add end separator if directory
            if ((asDirectory ?? DirectoryMode) && path[path.Length - 1] != separator)
                path += separator;

            return path;
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
