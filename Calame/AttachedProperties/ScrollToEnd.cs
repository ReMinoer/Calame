using System.Windows;
using System.Windows.Controls;
using Calame.Utils;

namespace Calame.AttachedProperties
{
    static public class ScrollToEnd
    {
        static public readonly DependencyProperty ScrollToEndRequestedProperty
            = DependencyProperty.RegisterAttached("ScrollToEndRequested", typeof(bool), typeof(ScrollToEnd), new UIPropertyMetadata(false, OnScrollToEndRequestedPropertyChanged));

        static public bool GetScrollToEndRequested(DependencyObject obj) => (bool)obj.GetValue(ScrollToEndRequestedProperty);
        static public void SetScrollToEndRequested(DependencyObject obj, bool value) => obj.SetValue(ScrollToEndRequestedProperty, value);

        static private void OnScrollToEndRequestedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
                return;

            var itemsControl = (ListView)d;

            int lastIndex = itemsControl.Items.Count - 1;
            if (lastIndex == -1)
                return;
            
            var lastControlItem = itemsControl.GetGeneratedItemContainer<FrameworkElement>(lastIndex);
            if (!lastControlItem.IsVisible)
                return;

            object lastItem = itemsControl.Items[lastIndex];
            itemsControl.ScrollIntoView(lastItem);
        }
    }
}