using System.Windows;

namespace Calame.AttachedProperties
{
    static public class Focus
    {
        static public readonly DependencyProperty IsFocusedProperty
            = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(Focus), new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        static public bool GetIsFocused(DependencyObject obj) => (bool)obj.GetValue(IsFocusedProperty);
        static public void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

        static private void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((UIElement)d).Focus();
            }
        }
    }
}