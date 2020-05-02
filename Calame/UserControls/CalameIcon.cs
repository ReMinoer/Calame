using System.Windows;
using System.Windows.Controls;
using Calame.Icons;

namespace Calame.UserControls
{
    public class CalameIcon : UserControl
    {
        static public readonly DependencyProperty IconDescriptionProperty =
            DependencyProperty.Register(nameof(IconDescription), typeof(IconDescription), typeof(CalameIcon), new PropertyMetadata(IconDescription.None, OnDescriptionChanged));

        static public readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(int), typeof(CalameIcon), new PropertyMetadata(32, OnRenderingChanged));
        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(CalameIcon), new PropertyMetadata(null, OnRenderingChanged));

        static public readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(nameof(Target), typeof(object), typeof(CalameIcon), new PropertyMetadata(null, OnDescriptionChanged));
        static public readonly DependencyProperty IconTargetSelectorProperty =
            DependencyProperty.Register(nameof(IconTargetSelector), typeof(IIconTargetSelector), typeof(CalameIcon), new PropertyMetadata(null, OnDescriptionChanged));
        static public readonly DependencyProperty IconDescriptorProperty =
            DependencyProperty.Register(nameof(IconDescriptor), typeof(IIconDescriptor), typeof(CalameIcon), new PropertyMetadata(null, OnDescriptionChanged));

        public IconDescription IconDescription
        {
            get => (IconDescription)GetValue(IconDescriptionProperty);
            set => SetValue(IconDescriptionProperty, value);
        }

        public object Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public int IconSize
        {
            get => (int)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public IIconDescriptor IconDescriptor
        {
            get => (IIconDescriptor)GetValue(IconDescriptorProperty);
            set => SetValue(IconDescriptorProperty, value);
        }

        public IIconTargetSelector IconTargetSelector
        {
            get => (IIconTargetSelector)GetValue(IconTargetSelectorProperty);
            set => SetValue(IconTargetSelectorProperty, value);
        }

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        private IconDescription _displayedIconDescription;
        private IconDescription DisplayedIconDescription
        {
            get => _displayedIconDescription;
            set
            {
                if (_displayedIconDescription.Equals(value))
                    return;

                _displayedIconDescription = value;
                OnRenderingChanged();
            }
        }

        static private void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CalameIcon)d).OnDescriptionChanged();
        static private void OnRenderingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CalameIcon)d).OnRenderingChanged();

        public CalameIcon()
        {
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        private void OnDescriptionChanged()
        {
            DisplayedIconDescription = IconDescription.Defined
                ? IconDescription
                : IconDescriptor?.GetIcon(IconTargetSelector?.GetIconTarget(Target) ?? Target) ?? IconDescription.None;
        }

        private void OnRenderingChanged()
        {
            Content = IconProvider?.GetControl(DisplayedIconDescription, IconSize);
        }
    }
}