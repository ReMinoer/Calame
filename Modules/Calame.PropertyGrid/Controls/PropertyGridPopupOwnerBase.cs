using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Calame.ContentFileTypes;
using Calame.Icons;
using Calame.Utils;
using Diese.Collections;
using Gemini.Framework;
using Glyph.Pipeline;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public delegate void ItemEventHandler(object sender, ItemEventArgs args);

    public class ItemEventArgs : EventArgs
    {
        public object Item { get; }
        public ItemEventArgs(object item) { Item = item; }
    }

    public abstract class PropertyGridPopupOwnerBase : UserControl, INotifyPropertyChanged
    {
        static public readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(false, OnIsReadOnlyChanged));
        static public readonly DependencyProperty IsReadOnlyValueProperty =
            DependencyProperty.Register(nameof(IsReadOnlyValue), typeof(bool), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(false));

        static public readonly DependencyProperty EditorDefinitionsProperty =
            DependencyProperty.Register(nameof(EditorDefinitions), typeof(EditorDefinitionCollection), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty NewItemTypeRegistryProperty =
            DependencyProperty.Register(nameof(NewItemTypeRegistry), typeof(IList<Type>), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null, OnNewItemTypeRegistryChanged));

        static public readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register(nameof(ShowHeader), typeof(bool), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(true));
        static public readonly DependencyProperty PopupWidthProperty =
            DependencyProperty.Register(nameof(PopupWidth), typeof(double), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(double.NaN));

        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty IconDescriptorProperty =
            DependencyProperty.Register(nameof(IconDescriptor), typeof(IIconDescriptor), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty IconDescriptorManagerProperty =
            DependencyProperty.Register(nameof(IconDescriptorManager), typeof(IIconDescriptorManager), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty SystemIconDescriptorProperty =
            DependencyProperty.Register(nameof(SystemIconDescriptor), typeof(IIconDescriptor), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty IconTargetSelectorProperty =
            DependencyProperty.Register(nameof(IconTargetSelector), typeof(IIconTargetSelector), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));

        static public readonly DependencyProperty WorkingDirectoryProperty =
            DependencyProperty.Register(nameof(WorkingDirectory), typeof(string), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));

        static public readonly DependencyProperty ContentFileTypeResolverProperty =
            DependencyProperty.Register(nameof(ContentFileTypeResolver), typeof(IContentFileTypeResolver), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty RawContentLibraryProperty =
            DependencyProperty.Register(nameof(RawContentLibrary), typeof(IRawContentLibrary), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));

        static public readonly DependencyProperty OpenFileCommandProperty =
            DependencyProperty.Register(nameof(OpenFileCommand), typeof(ICommand), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty OpenFolderCommandProperty =
            DependencyProperty.Register(nameof(OpenFolderCommand), typeof(ICommand), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty SelectItemCommandProperty =
            DependencyProperty.Register(nameof(SelectItemCommand), typeof(ICommand), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public bool IsReadOnlyValue
        {
            get => (bool)GetValue(IsReadOnlyValueProperty);
            set => SetValue(IsReadOnlyValueProperty, value);
        }

        public EditorDefinitionCollection EditorDefinitions
        {
            get => (EditorDefinitionCollection)GetValue(EditorDefinitionsProperty);
            set => SetValue(EditorDefinitionsProperty, value);
        }

        public IList<Type> NewItemTypeRegistry
        {
            get => (IList<Type>)GetValue(NewItemTypeRegistryProperty);
            set => SetValue(NewItemTypeRegistryProperty, value);
        }

        public bool ShowHeader
        {
            get => (bool)GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        public double PopupWidth
        {
            get => (double)GetValue(PopupWidthProperty);
            set => SetValue(PopupWidthProperty, value);
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

        public string WorkingDirectory
        {
            get => (string)GetValue(WorkingDirectoryProperty);
            set => SetValue(WorkingDirectoryProperty, value);
        }

        public IContentFileTypeResolver ContentFileTypeResolver
        {
            get => (IContentFileTypeResolver)GetValue(ContentFileTypeResolverProperty);
            set => SetValue(ContentFileTypeResolverProperty, value);
        }

        public IRawContentLibrary RawContentLibrary
        {
            get => (IRawContentLibrary)GetValue(RawContentLibraryProperty);
            set => SetValue(RawContentLibraryProperty, value);
        }

        public ICommand OpenFileCommand
        {
            get => (ICommand)GetValue(OpenFileCommandProperty);
            set => SetValue(OpenFileCommandProperty, value);
        }

        public ICommand OpenFolderCommand
        {
            get => (ICommand)GetValue(OpenFolderCommandProperty);
            set => SetValue(OpenFolderCommandProperty, value);
        }

        public ICommand SelectItemCommand
        {
            get => (ICommand)GetValue(SelectItemCommandProperty);
            set => SetValue(SelectItemCommandProperty, value);
        }

        public bool IsPropertyGridReadOnly => IsReadOnly || (IsReadOnlyValue && (PropertyGridDisplayedType?.IsValueType ?? false));
        protected Type PropertyGridDisplayedType { get; private set; }

        public ICommand ExpandObjectCommand { get; }
        public ICommand SelectOrExpandItemCommand { get; }

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

        public event PropertyValueChangedEventHandler PropertyValueChanged;

        protected PropertyGridPopupOwnerBase()
        {
            ExpandObjectCommand = new RelayCommand(OnExpandObject);
            SelectOrExpandItemCommand = new RelayCommand(OnSelectOrExpandItem);
            _addItemCommand = new RelayCommand(x => AddItem(CreateItem((Type)x)));
        }

        static private void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PropertyGridPopupOwnerBase)d;
            control.OnPropertyChanged(nameof(AddButtonEnabled));
            control.OnIsReadOnlyChanged(e);
        }

        static private void OnNewItemTypeRegistryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PropertyGridPopupOwnerBase)d;
            control.RefreshNewItemTypes();
            control.OnNewItemTypeRegistryChanged(e);
        }

        protected virtual void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e) {}
        protected virtual void OnNewItemTypeRegistryChanged(DependencyPropertyChangedEventArgs e) {}

        protected void RefreshNewItemTypes()
        {
            Type itemType = GetNewItemType();
            PropertyGridDisplayedType = itemType;

            if (itemType == null)
                return;

            IList<Type> newItemTypes = NewItemTypeRegistry?.Where(x => itemType.IsAssignableFrom(x)).ToList() ?? new List<Type>();
            if (!newItemTypes.Contains(itemType) && IsInstantiableWithoutParameter(itemType))
                newItemTypes.Insert(0, itemType);

            NewItemTypes = newItemTypes;
        }

        public abstract bool CanAddItem { get; }
        public abstract bool CanRemoveItem { get; }
        protected abstract void AddItem(object item);
        protected abstract void RemoveItem(DependencyObject popupOwner);
        protected abstract void RefreshValueType(DependencyObject popupOwner, object value);
        protected abstract Type GetNewItemType();

        static private bool IsInstantiableWithoutParameter(Type type)
        {
            if (type.IsValueType)
                return true;

            if (type.IsInterface)
                return false;
            if (type.IsAbstract)
                return false;
            if (type.IsGenericType && type.GetConstructor(type.GenericTypeArguments) != null)
                return false;
            if (type.GetConstructor(Type.EmptyTypes) == null)
                return false;

            return true;
        }

        protected virtual object CreateItem(Type type)
        {
            if (type.IsGenericType && type.GetConstructor(type.GenericTypeArguments) != null)
                return Activator.CreateInstance(type, type.GenericTypeArguments.Select(Activator.CreateInstance).ToArray());

            return Activator.CreateInstance(type);
        }

        protected void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_newItemTypes.Count == 1)
            {
                AddItem(CreateItem(_newItemTypes[0]));
                return;
            }

            var contextMenu = new ContextMenu
            {
                PlacementTarget = (UIElement)sender
            };

            string[] typeNames = _newItemTypes.Select(x => x.Name).ToArray();
            ReduceTypeNamePatterns(typeNames);

            for (int i = 0; i < _newItemTypes.Count; i++)
            {
                var menuItem = new MenuItem
                {
                    Header = typeNames[i],
                    Command = _addItemCommand,
                    CommandParameter = _newItemTypes[i],
                    Icon = IconProvider.GetControl(IconDescriptor.GetTypeIcon(_newItemTypes[i]), 16)
                };

                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }
        
        protected void OnSelectOrExpandItem(object control)
        {
            var popupOwner = (FrameworkElement)control;

            if (Keyboard.Modifiers != ModifierKeys.Control
                && SelectItemCommand != null && SelectItemCommand.CanExecute(popupOwner.DataContext))
                SelectItemCommand.Execute(popupOwner.DataContext);
            else
                ExpandObjectCommand.Execute(popupOwner);
        }

        protected void OnExpandObject(object control)
        {
            var popupOwner = (FrameworkElement)control;

            var popup = new PropertyGridPopup
            {
                PlacementTarget = popupOwner,
                StaysOpen = false
            };

            popup.SetBinding(PropertyGridPopup.WidthProperty, new Binding(nameof(PopupWidth)) { Source = this });
            popup.SetBinding(PropertyGridPopup.CanRemoveItemProperty, new Binding(nameof(CanRemoveItem)) { Source = this });
            popup.SetBinding(PropertyGridPopup.IconProviderProperty, new Binding(nameof(IconProvider)) { Source = this });
            popup.SetBinding(PropertyGridPopup.SystemIconDescriptorProperty, new Binding(nameof(SystemIconDescriptor)) { Source = this });

            CalamePropertyGrid propertyGrid = popup.PropertyGrid;
            propertyGrid.SetBinding(CalamePropertyGrid.NewItemTypeRegistryProperty, new Binding(nameof(NewItemTypeRegistry)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.ShowHeaderProperty, new Binding(nameof(ShowHeader)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.PopupsWidthProperty, new Binding(nameof(PopupWidth)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.IconProviderProperty, new Binding(nameof(IconProvider)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.IconDescriptorManagerProperty, new Binding(nameof(IconDescriptorManager)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.WorkingDirectoryProperty, new Binding(nameof(WorkingDirectory)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.ContentFileTypeResolverProperty, new Binding(nameof(ContentFileTypeResolver)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.RawContentLibraryProperty, new Binding(nameof(RawContentLibrary)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.OpenFileCommandProperty, new Binding(nameof(OpenFileCommand)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.OpenFolderCommandProperty, new Binding(nameof(OpenFolderCommand)) { Source = this });

            propertyGrid.WrapValueTypeObject = true;
            propertyGrid.SetBinding(CalamePropertyGrid.SelectedObjectProperty, new Binding(nameof(DataContext)) { Source = control });

            propertyGrid.IsReadOnly = IsPropertyGridReadOnly;

            var selectItemCommand = new RelayCommand(OnSelectItem, CanSelectItem);
            propertyGrid.SelectItemCommand = selectItemCommand;
            popup.SelectItemCommand = selectItemCommand;
            popup.CanSelectItem = selectItemCommand.CanExecute(propertyGrid.SelectedObject);

            bool CanSelectItem(object item) => SelectItemCommand?.CanExecute(item) ?? false;
            void OnSelectItem(object item)
            {
                popup.IsOpen = false;
                SelectItemCommand?.Execute(item);
            }

            popup.Removed += OnRemoved;
            propertyGrid.PropertyValueChanged += OnPropertyValueChanged;

            void OnRemoved(object sender, EventArgs e) => OnItemRemoved(popup, popupOwner);
            void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) => OnPopupPropertyValueChanged(propertyGrid, popupOwner, e);

            popup.Closed += (sender, args) =>
            {
                propertyGrid.PropertyValueChanged -= OnPropertyValueChanged;
                popup.Removed -= OnRemoved;

                popup.DataContext = null;
            };

            popup.IsOpen = true;

            if (propertyGrid.IsValueTypeObject)
            {
                // If value type object use InlineObjectControl, do not wrap it.
                FrameworkElement editor = propertyGrid.Properties.First().Editor;
                if (editor is InlineObjectControl)
                    propertyGrid.WrapValueTypeObject = false;
            }

            // Focus first control if value type object is wrapped.
            if (propertyGrid.IsValueTypeObject && propertyGrid.WrapValueTypeObject)
            {
                FrameworkElement editor = propertyGrid.Properties.First().Editor;
                UIElement firstFocusableEditorElement = Tree.BreadthFirst<UIElement>(editor, x => x.GetVisualChildren().OfType<UIElement>()).FirstOrDefault(x => x.Focusable);

                firstFocusableEditorElement?.Focus();
            }
        }

        private void OnItemRemoved(Popup sender, DependencyObject popupOwner)
        {
            sender.IsOpen = false;
            RemoveItem(popupOwner);
        }

        private void OnPopupPropertyValueChanged(CalamePropertyGrid propertyGrid, DependencyObject popupOwner, PropertyValueChangedEventArgs e)
        {
            if (propertyGrid.IsValueTypeObject)
                RefreshValueType(popupOwner, propertyGrid.EditedValueTypeValue);

            OnPropertyValueChanged(e);
        }

        protected void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
        {
            PropertyValueChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        static private void ReduceTypeNamePatterns(string[] values)
        {
            while (true)
            {
                int upperIndex = values[0].Skip(1).IndexOf(char.IsUpper) + 1;
                if (upperIndex <= 0)
                    break;

                string prefix = values[0].Substring(0, upperIndex);
                if (!values.Skip(1).All(x => x.StartsWith(prefix)))
                    break;

                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Substring(prefix.Length);
            }

            while (true)
            {
                int upperIndex = LastIndexOf(values[0], char.IsUpper);
                if (upperIndex == -1)
                    break;

                int suffixLength = values[0].Length - upperIndex;
                string suffix = values[0].Substring(upperIndex, suffixLength);
                if (!values.Skip(1).All(x => x.EndsWith(suffix)))
                    break;

                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Substring(0, values[i].Length - suffixLength);
            }
        }

        static public int LastIndexOf(string value, Predicate<char> predicate)
        {
            for (int i = value.Length - 1; i >= 0; i--)
                if (predicate(value[i]))
                    return i;
            return -1;
        }
    }
}