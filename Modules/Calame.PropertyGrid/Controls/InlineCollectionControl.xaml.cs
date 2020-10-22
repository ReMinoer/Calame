using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Calame.Icons;
using Diese.Collections;
using Gemini.Framework;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public partial class InlineCollectionControl : PropertyGridPopupOwnerBase, INotifyPropertyChanged
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
        public override bool IsItemsSourceResizable => _list != null && !_list.IsFixedSize;
        public bool IsItemsSourceEditable => _list != null && !_list.IsReadOnly;

        private Type _propertyGridDisplayedType;
        protected override Type PropertyGridDisplayedType => _propertyGridDisplayedType;

        private IList<Type> _newItemTypes;
        private IList<Type> NewItemTypes
        {
            get => _newItemTypes;
            set
            {
                if (_newItemTypes == value)
                    return;

                _newItemTypes = value;
                OnPropertyChanged(nameof(AddButtonIconKey));
                OnPropertyChanged(nameof(AddButtonEnabled));
                OnPropertyChanged(nameof(AddButtonTooltip));
            }
        }

        public CalameIconKey AddButtonIconKey => NewItemTypes != null && NewItemTypes.Count > 1 ? CalameIconKey.AddFromList : CalameIconKey.Add;
        public bool AddButtonEnabled => !IsReadOnly && NewItemTypes != null && NewItemTypes.Count > 0;
        public string AddButtonTooltip
        {
            get
            {
                if (NewItemTypes == null || NewItemTypes.Count == 0)
                    return "No types to add found";
                return NewItemTypes.Count == 1 ? $"Add {NewItemTypes[0].Name}" : "Add item...";
            }
        }

        private readonly RelayCommand _addItemCommand;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        static InlineCollectionControl()
        {
            NewItemTypeRegistryProperty.OverrideMetadata(typeof(InlineCollectionControl), new PropertyMetadata(null, OnNewItemTypeRegistryChanged));
            IsReadOnlyProperty.OverrideMetadata(typeof(InlineCollectionControl), new PropertyMetadata(false, IsReadOnlyChanged));
        }

        public InlineCollectionControl()
        {
            InitializeComponent();

            _addItemCommand = new RelayCommand(OnAddItem);
        }

        static private void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineCollectionControl)d;

            RefreshItemsSource(control, (IEnumerable)e.NewValue);
            RefreshNewItemTypes(control);
        }

        static private void OnNewItemTypeRegistryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineCollectionControl)d;

            RefreshNewItemTypes(control);
        }

        static private void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineCollectionControl)d;

            control.OnPropertyChanged(nameof(AddButtonEnabled));
        }

        static private void RefreshItemsSource(InlineCollectionControl control, IEnumerable itemsSource)
        {
            control._list = itemsSource as IList;
            if (!control.IsItemsSourceResizable)
                control._array = itemsSource as Array;

            control.OnPropertyChanged(nameof(IsItemsSourceResizable));
            control.OnPropertyChanged(nameof(IsItemsSourceEditable));
        }

        static private void RefreshNewItemTypes(InlineCollectionControl control)
        {
            if (!control.IsItemsSourceResizable)
            {
                control.NewItemTypes = null;
                control._propertyGridDisplayedType = null;
                return;
            }

            Type[] interfaces = control.ItemsSource.GetType().GetInterfaces();
            if (!interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>), out Type collectionType))
            {
                control.NewItemTypes = null;
                control._propertyGridDisplayedType = null;
                return;
            }

            Type itemType = collectionType.GenericTypeArguments[0];
            control._propertyGridDisplayedType = itemType;

            IList<Type> newItemTypes = control.NewItemTypeRegistry?.Where(x => itemType.IsAssignableFrom(x)).ToList() ?? new List<Type>();
            if (!newItemTypes.Contains(itemType) && IsInstantiableWithoutParameter(itemType))
                newItemTypes.Insert(0, itemType);

            control.NewItemTypes = newItemTypes;
        }

        static private bool IsInstantiableWithoutParameter(Type type)
        {
            if (type.IsValueType)
                return true;

            if (type.IsInterface)
                return false;
            if (type.IsAbstract)
                return false;
            if (type.IsGenericType)
                return false;
            if (type.GetConstructor(Type.EmptyTypes) == null)
                return false;

            return true;
        }

        private void OnPropertyCollectionChanged()
        {
            OnPropertyValueChanged(new PropertyValueChangedEventArgs(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PropertyValueChangedEvent, this, ItemsSource, ItemsSource));
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_newItemTypes.Count == 1)
            {
                OnAddItem(Activator.CreateInstance(_newItemTypes[0]));
                return;
            }

            var contextMenu = new ContextMenu
            {
                PlacementTarget = (UIElement)sender
            };

            foreach (Type newItemType in _newItemTypes)
            {
                object item = Activator.CreateInstance(newItemType);

                var menuItem = new MenuItem
                {
                    Header = newItemType.Name,
                    Command = _addItemCommand,
                    CommandParameter = item,
                    Icon = IconProvider.GetControl(IconDescriptor.GetIcon(item), 16)
                };

                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }

        private void OnAddItem(object item)
        {
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
            if (!IsItemsSourceEditable)
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

            if (IsItemsSourceResizable)
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
