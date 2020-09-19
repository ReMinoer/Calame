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
using Calame.Icons;
using Calame.Utils;
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

    public abstract class PropertyGridPopupOwnerBase : UserControl, INotifyPropertyChanged
    {
        static public readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(false));
        static public readonly DependencyProperty IsReadOnlyValueProperty =
            DependencyProperty.Register(nameof(IsReadOnlyValue), typeof(bool), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(false));

        static public readonly DependencyProperty EditorDefinitionsProperty =
            DependencyProperty.Register(nameof(EditorDefinitions), typeof(EditorDefinitionCollection), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty NewItemTypeRegistryProperty =
            DependencyProperty.Register(nameof(NewItemTypeRegistry), typeof(IList<Type>), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));

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

        static public readonly DependencyProperty OpenFileCommandProperty =
            DependencyProperty.Register(nameof(OpenFileCommand), typeof(ICommand), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));
        static public readonly DependencyProperty OpenFolderCommandProperty =
            DependencyProperty.Register(nameof(OpenFolderCommand), typeof(ICommand), typeof(PropertyGridPopupOwnerBase), new PropertyMetadata(null));

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

        public bool IsPropertyGridReadOnly => IsReadOnly || (IsReadOnlyValue && (PropertyGridDisplayedType?.IsValueType ?? false));
        public ICommand ExpandObjectCommand { get; }

        public event ItemEventHandler ItemSelected;
        public event PropertyValueChangedEventHandler PropertyValueChanged;

        protected PropertyGridPopupOwnerBase()
        {
            ExpandObjectCommand = new RelayCommand(OnExpandObject);
        }

        protected void OnExpandObject(object control)
        {
            var popupOwner = (FrameworkElement)control;
            object itemModel = popupOwner.DataContext;

            var popup = new PropertyGridPopup
            {
                PlacementTarget = popupOwner,
                StaysOpen = false
            };

            popup.SetBinding(PropertyGridPopup.WidthProperty, new Binding(nameof(PopupWidth)) { Source = this });
            popup.SetBinding(PropertyGridPopup.CanRemoveItemProperty, new Binding(nameof(IsItemsSourceResizable)) { Source = this });
            popup.SetBinding(PropertyGridPopup.IconProviderProperty, new Binding(nameof(IconProvider)) { Source = this });
            popup.SetBinding(PropertyGridPopup.SystemIconDescriptorProperty, new Binding(nameof(SystemIconDescriptor)) { Source = this });

            CalamePropertyGrid propertyGrid = popup.PropertyGrid;
            propertyGrid.SetBinding(CalamePropertyGrid.NewItemTypeRegistryProperty, new Binding(nameof(NewItemTypeRegistry)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.ShowHeaderProperty, new Binding(nameof(ShowHeader)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.PopupsWidthProperty, new Binding(nameof(PopupWidth)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.IconProviderProperty, new Binding(nameof(IconProvider)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.IconDescriptorManagerProperty, new Binding(nameof(IconDescriptorManager)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.OpenFileCommandProperty, new Binding(nameof(OpenFileCommand)) { Source = this });
            propertyGrid.SetBinding(CalamePropertyGrid.OpenFolderCommandProperty, new Binding(nameof(OpenFolderCommand)) { Source = this });

            propertyGrid.WrapValueTypeObject = true;
            propertyGrid.SetBinding(CalamePropertyGrid.SelectedObjectProperty, new Binding(nameof(DataContext)) { Source = control });

            propertyGrid.IsReadOnly = IsPropertyGridReadOnly;
            popup.CanSelectItem = !propertyGrid.IsValueTypeObject;

            popup.Removed += OnRemoved;
            popup.Selected += OnSelected;
            propertyGrid.PropertyValueChanged += OnPropertyValueChanged;

            void OnRemoved(object sender, EventArgs e) => OnItemRemoved(popup, popupOwner);
            void OnSelected(object sender, EventArgs e) => OnItemSelected(popup, itemModel);
            void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) => OnPopupPropertyValueChanged(propertyGrid, popupOwner, e);

            popup.Closed += (sender, args) =>
            {
                propertyGrid.PropertyValueChanged -= OnPropertyValueChanged;
                popup.Selected -= OnSelected;
                popup.Removed -= OnRemoved;
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
            OnItemRemoved(popupOwner);
        }

        private void OnItemSelected(Popup sender, object item)
        {
            sender.IsOpen = false;
            ItemSelected?.Invoke(this, new ItemEventArgs(item));
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

        public abstract bool IsItemsSourceResizable { get; }
        protected abstract Type PropertyGridDisplayedType { get; }
        protected abstract void OnItemRemoved(DependencyObject popupOwner);
        protected abstract void RefreshValueType(DependencyObject popupOwner, object value);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}