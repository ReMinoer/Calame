using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Diese.Collections;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public partial class InlineCollectionControl : PropertyGridPopupOwnerBase
    {
        static public readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(InlineCollectionControl), new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private IList _list;
        private Array _array;
        public override bool CanAddItem => _list != null && !_list.IsFixedSize;
        public override bool CanRemoveItem => CanAddItem;
        public bool CanEditItem => _list != null && !_list.IsReadOnly;

        public InlineCollectionControl()
        {
            InitializeComponent();
        }

        static private void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineCollectionControl)d;

            RefreshItemsSource(control, (IEnumerable)e.NewValue);
            control.RefreshNewItemTypes();
        }

        static private void RefreshItemsSource(InlineCollectionControl control, IEnumerable itemsSource)
        {
            control._list = itemsSource as IList;
            if (!control.CanAddItem)
                control._array = itemsSource as Array;

            control.OnPropertyChanged(nameof(CanAddItem));
            control.OnPropertyChanged(nameof(CanRemoveItem));
            control.OnPropertyChanged(nameof(CanEditItem));
        }

        protected override Type GetNewItemType()
        {
            if (!CanAddItem)
                return null;

            Type[] interfaces = ItemsSource.GetType().GetInterfaces();
            if (!interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>), out Type collectionType))
                return null;

            return collectionType.GenericTypeArguments[0];
        }

        protected override void OnAddItem(object type)
        {
            object item = Activator.CreateInstance((Type)type);
            _list.Add(item);

            OnPropertyCollectionChanged();
            OnExpandObject(ItemsControl.ItemContainerGenerator.ContainerFromItem(item));
        }

        protected override void OnItemRemoved(DependencyObject popupOwner)
        {
            int itemIndex = GetIndex(popupOwner);
            if (itemIndex == -1)
                throw new InvalidOperationException();

            (_list[itemIndex] as IDisposable)?.Dispose();

            _list.RemoveAt(itemIndex);
            OnPropertyCollectionChanged();
        }

        private void OnPropertyCollectionChanged()
        {
            OnPropertyValueChanged(new PropertyValueChangedEventArgs(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PropertyValueChangedEvent, this, ItemsSource, ItemsSource));
        }

        protected override void RefreshValueType(DependencyObject popupOwner, object value)
        {
            int currentIndex = GetIndex(popupOwner);
            if (currentIndex == -1)
                throw new InvalidOperationException();

            _list[currentIndex] = value;
        }

        private FrameworkElement _dragSender;
        private Point? _dragStartPosition;
        private const int DragDelta = 10;

        private void OnItemPreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            _dragSender = (FrameworkElement)sender;
            _dragStartPosition = e.GetPosition(_dragSender);
        }

        private void OnItemPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            if (!CanEditItem)
                return;
            if (_dragStartPosition == null)
                return;
            if (sender != _dragSender)
                return;

            Point dragCurrentPosition = e.GetPosition(_dragSender);
            if ((dragCurrentPosition - _dragStartPosition.Value).Length < DragDelta)
                return;

            _dragStartPosition = null;

            int currentIndex = GetIndex(_dragSender);
            if (currentIndex == -1)
                throw new InvalidOperationException();

            var data = new DataObject();
            data.SetData(nameof(DraggedItem), new DraggedItem(this, _dragSender.DataContext, currentIndex));
            DragDrop.DoDragDrop(_dragSender, data, DragDropEffects.Move);
            e.Handled = true;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            DraggedItem draggedItem = GetDraggedItem(e);
            if (draggedItem != null)
                e.Effects = DragDropEffects.Move;

            e.Handled = true;
        }

        private void OnItemDrop(object sender, DragEventArgs e)
        {
            DraggedItem draggedItem = GetDraggedItem(e);
            if (draggedItem == null)
                return;

            int newIndex = GetIndex((DependencyObject)sender);
            if (newIndex == -1)
                throw new InvalidOperationException();

            if (newIndex == draggedItem.Index)
                return;

            if (CanAddItem)
            {
                _list.RemoveAt(draggedItem.Index);
                _list.Insert(newIndex, draggedItem.Data);
            }
            else
            {
                if (_array != null)
                {
                    if (newIndex < draggedItem.Index)
                    {
                        Array.Copy(_array, newIndex, _array, newIndex + 1, draggedItem.Index - newIndex);
                    }
                    else // if (draggedItem.Index < newIndex)
                    {
                        Array.Copy(_array, draggedItem.Index + 1, _array, draggedItem.Index, newIndex - draggedItem.Index);
                    }
                }
                else
                {
                    if (newIndex < draggedItem.Index)
                    {
                        for (int i = draggedItem.Index; i > newIndex; i--)
                            _list[i] = _list[i - 1];
                    }
                    else // if (draggedItem.Index < newIndex)
                    {
                        for (int i = draggedItem.Index; i < newIndex; i++)
                            _list[i] = _list[i + 1];
                    }
                }
                _list[newIndex] = draggedItem.Data;
            }

            OnPropertyCollectionChanged();
        }

        private DraggedItem GetDraggedItem(DragEventArgs dragEventArgs)
        {
            if (!dragEventArgs.Data.GetDataPresent(nameof(DraggedItem)))
                return null;
            if (!(dragEventArgs.Data.GetData(nameof(DraggedItem)) is DraggedItem draggedItem))
                return null;
            if (draggedItem.Owner != this)
                return null;

            return draggedItem;
        }

        static private int GetIndex(DependencyObject dependencyObject)
        {
            while (dependencyObject != null && !(dependencyObject is ContentPresenter))
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);

            return dependencyObject != null ? ItemsControl.GetAlternationIndex(dependencyObject) : -1;
        }

        private class DraggedItem
        {
            public object Owner { get; }
            public object Data { get; }
            public int Index { get; }

            public DraggedItem(object owner, object data, int index = -1)
            {
                Owner = owner;
                Data = data;
                Index = index;
            }
        }
    }
}
