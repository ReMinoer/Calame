using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Calame.Icons;

namespace Calame.UserControls
{
    public class CalameIcon : UserControl
    {
        static public readonly DependencyProperty IconKeyProperty =
            DependencyProperty.Register(nameof(IconKey), typeof(object), typeof(CalameIcon), new PropertyMetadata(OnIconChanged));

        public object IconKey
        {
            get => GetValue(IconKeyProperty);
            set => SetValue(IconKeyProperty, value);
        }

        static public readonly DependencyProperty IconBrushProperty =
            DependencyProperty.Register(nameof(IconBrush), typeof(Brush), typeof(CalameIcon), new PropertyMetadata(OnIconChanged));

        public Brush IconBrush
        {
            get => (Brush)GetValue(IconBrushProperty);
            set => SetValue(IconBrushProperty, value);
        }

        static public readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(int), typeof(CalameIcon), new PropertyMetadata(OnIconChanged));

        public int IconSize
        {
            get => (int)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(CalameIcon), new PropertyMetadata(OnIconChanged));

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        static private void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var calameIcon = (CalameIcon)d;

            var iconDescription = new IconDescription(calameIcon.IconKey, calameIcon.IconBrush);
            calameIcon.Content = calameIcon.IconProvider?.GetControl(iconDescription, calameIcon.IconSize);
        }
    }
}