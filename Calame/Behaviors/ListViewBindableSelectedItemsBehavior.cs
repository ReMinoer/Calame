using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Diese.Collections;
using Microsoft.Xaml.Behaviors;

namespace Calame.Behaviors
{
    public class ListViewBindableSelectedItemsBehavior : Behavior<ListView>
    {
        static public readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(ListViewBindableSelectedItemsBehavior), new UIPropertyMetadata(null, OnSelectedItemsChanged));

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        static private void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = (sender as ListViewBindableSelectedItemsBehavior)?.AssociatedObject;
            if (listView == null)
                return;

            listView.SelectedItems.Clear();
            listView.SelectedItems.AddMany((IList)e.NewValue);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
            base.OnDetaching();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems == null)
                return;

            SelectedItems.RemoveMany(e.RemovedItems);
            SelectedItems.AddMany(e.AddedItems);
        }
    }
}