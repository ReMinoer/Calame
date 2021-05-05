using System.Windows;
using System.Windows.Controls;
using Calame.Utils;
using Microsoft.Xaml.Behaviors;

namespace Calame.Behaviors
{
    public class TreeViewBindableSelectedItemBehavior : Behavior<TreeView>
    {
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        static public readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(TreeViewBindableSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemChanged));

        static private void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeView treeView = (sender as TreeViewBindableSelectedItemBehavior)?.AssociatedObject;
            
            TreeViewItem item = GetTreeViewItem(treeView, e.NewValue);
            if (item == null)
            {
                TreeViewItem oldItem = GetTreeViewItem(treeView, e.OldValue);
                if (oldItem != null)
                    oldItem.IsSelected = false;
                return;
            }

            item.IsSelected = true;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;

            base.OnDetaching();
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }

        // Edited from https://msdn.microsoft.com/en-gb/library/ff407130(v=vs.110).aspx

        static private TreeViewItem GetTreeViewItem(ItemsControl container, object item)
        {
            if (container != null)
            {
                if (container.DataContext == item)
                {
                    return container as TreeViewItem;
                }

                // Expand the current container
                if (container is TreeViewItem treeViewItem && !treeViewItem.IsExpanded)
                {
                    treeViewItem.SetValue(TreeViewItem.IsExpandedProperty, true);
                }

                foreach (TreeViewItem subContainer in container.GetAllGeneratedItemContainers<TreeViewItem>())
                {
                    // Store expand state
                    bool previouslyExpanded = subContainer.IsExpanded;

                    // Search the next level for the object.
                    TreeViewItem resultContainer = GetTreeViewItem(subContainer, item);
                    if (resultContainer != null)
                        return resultContainer;

                    // The object is not under this TreeViewItem so restore expand state.
                    subContainer.IsExpanded = previouslyExpanded;
                }
            }

            return null;
        }
    }
}