using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calame.Utils
{
    static public class ItemsControlExtension
    {
        static public T GetGeneratedItemContainer<T>(this ItemsControl itemsControl, int index)
            where T : FrameworkElement
        {
            PrepareItemsControl(itemsControl, out VirtualizingStackPanel virtualizingStackPanel);

            return GetItemContainerAtIndex<T>(index, itemsControl, virtualizingStackPanel);
        }

        static public IEnumerable<T> GetAllGeneratedItemContainers<T>(this ItemsControl itemsControl)
            where T : FrameworkElement
        {
            PrepareItemsControl(itemsControl, out VirtualizingStackPanel virtualizingStackPanel);

            for (int i = 0, count = itemsControl.Items.Count; i < count; i++)
            {
                var subContainer = GetItemContainerAtIndex<T>(i, itemsControl, virtualizingStackPanel);
                if (subContainer != null)
                    yield return subContainer;
            }
        }

        static private void PrepareItemsControl(ItemsControl itemsControl, out VirtualizingStackPanel virtualizingStackPanel)
        {
            // Try to generate the ItemsPresenter and the ItemsPanel.
            // by calling ApplyTemplate.  Note that in the 
            // virtualizing case even if the item is marked 
            // expanded we still need to do this step in order to 
            // regenerate the visuals because they may have been virtualized away.

            itemsControl.ApplyTemplate();
            var itemsPresenter = (ItemsPresenter)itemsControl.Template.FindName("ItemsHost", itemsControl);
            if (itemsPresenter != null)
            {
                itemsPresenter.ApplyTemplate();
            }
            else
            {
                // The Tree template has not named the ItemsPresenter, 
                // so walk the descendents and find the child.
                itemsPresenter = FindVisualChild<ItemsPresenter>(itemsControl);
                if (itemsPresenter == null)
                {
                    itemsControl.UpdateLayout();

                    itemsPresenter = FindVisualChild<ItemsPresenter>(itemsControl);
                }
            }

            var itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

            // Ensure that the generator for this panel has been created.
            UIElementCollection _ = itemsHostPanel.Children;

            virtualizingStackPanel = itemsHostPanel as VirtualizingStackPanel;
        }

        static private T GetItemContainerAtIndex<T>(int index, ItemsControl itemsControl, VirtualizingStackPanel virtualizingStackPanel)
            where T : FrameworkElement
        {
            T subContainer;
            if (virtualizingStackPanel != null)
            {
                // Bring the item into view so that the container will be generated.
                virtualizingStackPanel.BringIntoView(index);

                subContainer = (T)itemsControl.ItemContainerGenerator.ContainerFromIndex(index);
            }
            else
            {
                subContainer = (T)itemsControl.ItemContainerGenerator.ContainerFromIndex(index);

                // Bring the item into view to maintain the same behavior as with a virtualizing panel.
                subContainer.BringIntoView();
            }

            return subContainer;
        }

        static private T FindVisualChild<T>(Visual visual)
            where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(visual, i);
                if (child is T correctlyTyped)
                {
                    return correctlyTyped;
                }

                var descendent = FindVisualChild<T>((Visual)child);
                if (descendent != null)
                {
                    return descendent;
                }
            }

            return null;
        }
    }
}