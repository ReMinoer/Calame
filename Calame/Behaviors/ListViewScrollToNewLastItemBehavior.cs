using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Calame.Utils;
using Microsoft.Xaml.Behaviors;

namespace Calame.Behaviors
{
    public class ListViewScrollToNewLastItemBehavior : Behavior<ListView>
    {
        static public readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(ListViewScrollToNewLastItemBehavior), new UIPropertyMetadata(true));

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }
        static public readonly DependencyProperty OnlyIfAlreadyScrolledToEndProperty =
            DependencyProperty.Register(nameof(OnlyIfAlreadyScrolledToEnd), typeof(bool), typeof(ListViewScrollToNewLastItemBehavior), new UIPropertyMetadata(false));

        public bool OnlyIfAlreadyScrolledToEnd
        {
            get => (bool)GetValue(OnlyIfAlreadyScrolledToEndProperty);
            set => SetValue(OnlyIfAlreadyScrolledToEndProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            INotifyCollectionChanged items = AssociatedObject.Items;
            items.CollectionChanged += OnItemsSourceCollectionChanged;
        }

        protected override void OnDetaching()
        {
            INotifyCollectionChanged items = AssociatedObject.Items;
            items.CollectionChanged -= OnItemsSourceCollectionChanged;

            base.OnDetaching();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsEnabled)
                return;
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            if (OnlyIfAlreadyScrolledToEnd)
            {
                var border = VisualTreeHelper.GetChild(AssociatedObject, 0) as Decorator;
                var scrollViewer = border?.Child as ScrollViewer;
                if (scrollViewer == null)
                    return;

                if (scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight)
                    return;
            }

            int lastIndex = AssociatedObject.Items.Count - 1;
            if (lastIndex == -1)
                return;

            var lastControlItem = AssociatedObject.GetGeneratedItemContainer<FrameworkElement>(lastIndex);
            if (!lastControlItem.IsVisible)
                return;

            object lastItem = AssociatedObject.Items[lastIndex];
            AssociatedObject.ScrollIntoView(lastItem);
        }
    }
}