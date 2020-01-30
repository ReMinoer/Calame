using System.Windows;
using System.Windows.Controls;
using Calame.Icons;

namespace Calame.UserControls
{
    public class CalameIcon : UserControl
    {
        static public readonly DependencyProperty IconDescriptionProperty =
            DependencyProperty.Register(nameof(IconDescription), typeof(IconDescription), typeof(CalameIcon), new PropertyMetadata(OnIconChanged));
        static public readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(int), typeof(CalameIcon), new PropertyMetadata(OnIconChanged));
        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(CalameIcon), new PropertyMetadata(OnIconChanged));

        public IconDescription IconDescription
        {
            get => (IconDescription)GetValue(IconDescriptionProperty);
            set => SetValue(IconDescriptionProperty, value);
        }

        public int IconSize
        {
            get => (int)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        static private void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var calameIcon = (CalameIcon)d;
            calameIcon.Content = calameIcon.IconProvider?.GetControl(calameIcon.IconDescription, calameIcon.IconSize);
        }
    }
}