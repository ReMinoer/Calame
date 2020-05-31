using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Calame.Converters;
using Calame.Icons;
using Diese.Collections;
using Gemini.Framework;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public delegate void ItemEventHandler(object sender, ItemEventArgs args);

    public class ItemEventArgs : EventArgs
    {
        public object Item { get; }
        public ItemEventArgs(object item) { Item = item; }
    }

    public partial class InlineCollectionControl : INotifyPropertyChanged
    {
        static public readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(InlineCollectionControl), new PropertyMetadata(null, OnItemsSourceChanged));
        static public readonly DependencyProperty NewItemTypeRegistryProperty =
            DependencyProperty.Register(nameof(NewItemTypeRegistry), typeof(IList<Type>), typeof(InlineCollectionControl), new PropertyMetadata(null, OnNewItemTypeRegistryChanged));

        static public readonly DependencyProperty EditorDefinitionsProperty =
            DependencyProperty.Register(nameof(EditorDefinitions), typeof(EditorDefinitionCollection), typeof(InlineCollectionControl), new PropertyMetadata(null));

        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(InlineCollectionControl), new PropertyMetadata(null));
        static public readonly DependencyProperty IconDescriptorProperty =
            DependencyProperty.Register(nameof(IconDescriptor), typeof(IIconDescriptor), typeof(InlineCollectionControl), new PropertyMetadata(null));
        static public readonly DependencyProperty IconDescriptorManagerProperty =
            DependencyProperty.Register(nameof(IconDescriptorManager), typeof(IIconDescriptorManager), typeof(InlineCollectionControl), new PropertyMetadata(null));
        static public readonly DependencyProperty SystemIconDescriptorProperty =
            DependencyProperty.Register(nameof(SystemIconDescriptor), typeof(IIconDescriptor), typeof(InlineCollectionControl), new PropertyMetadata(null));
        static public readonly DependencyProperty IconTargetSelectorProperty =
            DependencyProperty.Register(nameof(IconTargetSelector), typeof(IIconTargetSelector), typeof(InlineCollectionControl), new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IList<Type> NewItemTypeRegistry
        {
            get => (IList<Type>)GetValue(NewItemTypeRegistryProperty);
            set => SetValue(NewItemTypeRegistryProperty, value);
        }

        public EditorDefinitionCollection EditorDefinitions
        {
            get => (EditorDefinitionCollection)GetValue(EditorDefinitionsProperty);
            set => SetValue(EditorDefinitionsProperty, value);
        }

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        public IIconDescriptorManager IconDescriptorManager
        {
            get => (IIconDescriptorManager)GetValue(IconDescriptorManagerProperty);
            set => SetValue(IconDescriptorManagerProperty, value);
        }

        public IIconDescriptor IconDescriptor
        {
            get => (IIconDescriptor)GetValue(IconDescriptorProperty);
            set => SetValue(IconDescriptorProperty, value);
        }

        public IIconDescriptor SystemIconDescriptor
        {
            get => (IIconDescriptor)GetValue(SystemIconDescriptorProperty);
            set => SetValue(SystemIconDescriptorProperty, value);
        }

        public IIconTargetSelector IconTargetSelector
        {
            get => (IIconTargetSelector)GetValue(IconTargetSelectorProperty);
            set => SetValue(IconTargetSelectorProperty, value);
        }

        private IList _list;
        private Array _array;
        public bool IsItemsSourceResizable => _list != null && !_list.IsFixedSize;
        public bool IsItemsSourceEditable => _list != null && !_list.IsReadOnly;

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

        public SystemIconKey AddButtonIconKey => NewItemTypes != null && NewItemTypes.Count > 1 ? SystemIconKey.AddFromList : SystemIconKey.Add;
        public bool AddButtonEnabled => NewItemTypes != null && NewItemTypes.Count > 0;
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
        public ICommand ItemClickedCommand { get; }

        public event ItemEventHandler ShowItemInPropertyGrid;
        public event PropertyValueChangedEventHandler PropertyValueChanged;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public InlineCollectionControl()
        {
            InitializeComponent();

            _addItemCommand = new RelayCommand(OnAddItem);
            ItemClickedCommand = new RelayCommand(OnItemClicked);
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
                return;
            }

            Type[] interfaces = control.ItemsSource.GetType().GetInterfaces();
            if (!interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>), out Type collectionType))
            {
                control.NewItemTypes = null;
                return;
            }

            Type itemType = collectionType.GenericTypeArguments[0];

            IList<Type> newItemTypes = control.NewItemTypeRegistry?.Where(x => itemType.IsAssignableFrom(x)).ToList() ?? new List<Type>();
            if (!newItemTypes.Contains(itemType) && (itemType.IsValueType || (!itemType.IsInterface && !itemType.IsAbstract && !itemType.IsGenericType && itemType.GetConstructor(Type.EmptyTypes) != null)))
                newItemTypes.Insert(0, itemType);

            control.NewItemTypes = newItemTypes;
        }

        private void OnShowItemInPropertyGrid(object item)
        {
            ShowItemInPropertyGrid?.Invoke(this, new ItemEventArgs(item));
        }

        private void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            PropertyValueChanged?.Invoke(this, e);
        }

        private void OnPropertyCollectionChanged()
        {
            PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PropertyValueChangedEvent, this, ItemsSource, ItemsSource));
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_newItemTypes.Count == 1)
            {
                OnAddItem(_newItemTypes[0]);
                return;
            }

            var contextMenu = new ContextMenu
            {
                PlacementTarget = (UIElement)sender
            };

            foreach (Type newItemType in _newItemTypes)
            {
                var item = new MenuItem
                {
                    Header = newItemType.Name,
                    Command = _addItemCommand,
                    CommandParameter = newItemType
                };

                contextMenu.Items.Add(item);
            }

            contextMenu.IsOpen = true;
        }

        private void OnAddItem(object parameter) => OnAddItem((Type)parameter);
        private void OnAddItem(Type itemType)
        {
            _list.Add(Activator.CreateInstance(itemType));
            OnPropertyCollectionChanged();
        }

        private void OnItemClicked(object control)
        {
            var frameworkElement = (FrameworkElement)control;
            object itemModel = frameworkElement.DataContext;

            var popup = new PropertyGridPopup
            {
                PlacementTarget = frameworkElement,
                Width = 300,
                StaysOpen = false
            };

            popup.SetBinding(PropertyGridPopup.CanRemoveItemProperty, new Binding(nameof(IsItemsSourceResizable)) { Source = this });
            popup.SetBinding(PropertyGridPopup.IconProviderProperty, new Binding(nameof(IconProvider)) { Source = this });
            popup.SetBinding(PropertyGridPopup.SystemIconDescriptorProperty, new Binding(nameof(SystemIconDescriptor)) { Source = this });

            CalamePropertyGrid propertyGrid = popup.PropertyGrid;
            propertyGrid.SetBinding(CalamePropertyGrid.SelectedObjectProperty, new Binding(nameof(DataContext)) { Source = control });
            propertyGrid.SetBinding(CalamePropertyGrid.NewItemTypeRegistryProperty, new Binding(nameof(NewItemTypeRegistry)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.IconProviderProperty, new Binding(nameof(IconProvider)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.IconDescriptorManagerProperty, new Binding(nameof(IconDescriptorManager)) { Source = this });

            void OnRemoved(object sender, EventArgs e) => OnItemRemoved(popup, itemModel);
            void OnShowInPropertyGrid(object sender, EventArgs e) => OnShowItemInPropertyGrid(itemModel);

            popup.Removed += OnRemoved;
            popup.ShowInPropertyGrid += OnShowInPropertyGrid;
            propertyGrid.PropertyValueChanged += OnPropertyValueChanged;

            popup.Unloaded += (sender, args) =>
            {
                propertyGrid.PropertyValueChanged -= OnPropertyValueChanged;
                popup.ShowInPropertyGrid -= OnShowInPropertyGrid;
                popup.Removed -= OnRemoved;
            };

            popup.IsOpen = true;
        }

        private void OnItemRemoved(Popup popup, object item)
        {
            popup.IsOpen = false;

            _list.Remove(item);
            OnPropertyCollectionChanged();
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

        private int GetIndex(DependencyObject dependencyObject)
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
