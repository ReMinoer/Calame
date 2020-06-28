using System;
using System.Windows;
using System.Windows.Navigation;
using Calame.Icons;

namespace Calame.PropertyGrid.Controls
{
    /// <summary>
    /// Interaction logic for PropertyGridPopup.xaml
    /// </summary>
    public partial class PropertyGridPopup
    {
        static public readonly DependencyProperty CanRemoveItemProperty =
            DependencyProperty.Register(nameof(CanRemoveItem), typeof(bool), typeof(PropertyGridPopup), new PropertyMetadata(true));
        static public readonly DependencyProperty CanSelectItemProperty =
            DependencyProperty.Register(nameof(CanSelectItem), typeof(bool), typeof(PropertyGridPopup), new PropertyMetadata(true));
        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(PropertyGridPopup), new PropertyMetadata(null));
        static public readonly DependencyProperty SystemIconDescriptorProperty =
            DependencyProperty.Register(nameof(SystemIconDescriptor), typeof(IIconDescriptor), typeof(PropertyGridPopup), new PropertyMetadata(null));

        public bool CanRemoveItem
        {
            get => (bool)GetValue(CanRemoveItemProperty);
            set => SetValue(CanRemoveItemProperty, value);
        }

        public bool CanSelectItem
        {
            get => (bool)GetValue(CanSelectItemProperty);
            set => SetValue(CanSelectItemProperty, value);
        }

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        public IIconDescriptor SystemIconDescriptor
        {
            get => (IIconDescriptor)GetValue(SystemIconDescriptorProperty);
            set => SetValue(SystemIconDescriptorProperty, value);
        }

        public event EventHandler Removed;
        public event EventHandler Selected;

        public PropertyGridPopup()
        {
            InitializeComponent();
        }

        private void OnDelete(object sender, RoutedEventArgs e) => Removed?.Invoke(this, EventArgs.Empty);
        private void OnSelect(object sender, RoutedEventArgs e) => Selected?.Invoke(this, EventArgs.Empty);
    }
}
