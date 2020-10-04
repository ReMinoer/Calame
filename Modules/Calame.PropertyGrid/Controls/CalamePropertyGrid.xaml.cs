using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Calame.ContentFileTypes;
using Calame.Icons;
using Calame.PropertyGrid.Utils;
using Diese;
using Glyph.Composition;
using Glyph.Pipeline;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public partial class CalamePropertyGrid : INotifyPropertyChanged
    {
        static public readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(CalamePropertyGrid), new PropertyMetadata(null, OnSelectedObjectChanged));
        static public readonly DependencyProperty WrapValueTypeObjectProperty =
            DependencyProperty.Register(nameof(WrapValueTypeObject), typeof(bool), typeof(CalamePropertyGrid), new PropertyMetadata(false, OnSelectedObjectChanged));
        static public readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(CalamePropertyGrid), new PropertyMetadata(false));

        static public readonly DependencyProperty NewItemTypeRegistryProperty =
            DependencyProperty.Register(nameof(NewItemTypeRegistry), typeof(IList<Type>), typeof(CalamePropertyGrid), new PropertyMetadata(null));

        static public readonly DependencyProperty CompactModeProperty =
            DependencyProperty.Register(nameof(CompactMode), typeof(bool), typeof(CalamePropertyGrid), new PropertyMetadata(false));
        static public readonly DependencyProperty ShowNavigationButtonsProperty =
            DependencyProperty.Register(nameof(ShowNavigationButtons), typeof(bool), typeof(CalamePropertyGrid), new PropertyMetadata(false));
        static public readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register(nameof(ShowHeader), typeof(bool), typeof(CalamePropertyGrid), new PropertyMetadata(true));
        static public readonly DependencyProperty PopupsWidthProperty =
            DependencyProperty.Register(nameof(PopupsWidth), typeof(double), typeof(CalamePropertyGrid), new PropertyMetadata(double.NaN));

        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        static public readonly DependencyProperty IconDescriptorManagerProperty =
            DependencyProperty.Register(nameof(IconDescriptorManager), typeof(IIconDescriptorManager), typeof(CalamePropertyGrid), new PropertyMetadata(null, OnIconDescriptorManagerChanged));

        static public readonly DependencyProperty WorkingDirectoryProperty =
            DependencyProperty.Register(nameof(WorkingDirectory), typeof(string), typeof(CalamePropertyGrid), new PropertyMetadata(null));

        static public readonly DependencyProperty ContentFileTypeResolverProperty =
            DependencyProperty.Register(nameof(ContentFileTypeResolver), typeof(IContentFileTypeResolver), typeof(CalamePropertyGrid), new PropertyMetadata(null, OnContentFileTypeResolverChanged));
        static public readonly DependencyProperty RawContentLibraryProperty =
            DependencyProperty.Register(nameof(RawContentLibrary), typeof(IRawContentLibrary), typeof(CalamePropertyGrid), new PropertyMetadata(null, OnRawContentLibraryChanged));

        static public readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register(nameof(PreviousCommand), typeof(ICommand), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        static public readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register(nameof(NextCommand), typeof(ICommand), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        static public readonly DependencyProperty OpenFileCommandProperty =
            DependencyProperty.Register(nameof(OpenFileCommand), typeof(ICommand), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        static public readonly DependencyProperty OpenFolderCommandProperty =
            DependencyProperty.Register(nameof(OpenFolderCommand), typeof(ICommand), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        
        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        private object _displayedObject;
        public object DisplayedObject
        {
            get => _displayedObject;
            private set
            {
                if (_displayedObject == value)
                    return;

                _displayedObject = value;
                OnPropertyChanged();
            }
        }

        private bool _isValueTypeObject;
        public bool IsValueTypeObject
        {
            get => _isValueTypeObject;
            private set
            {
                if (_isValueTypeObject == value)
                    return;

                _isValueTypeObject = value;
                OnPropertyChanged();
            }
        }

        private Type _valueType;
        private IValueTypeObject _valueTypeObject;

        private object _editedValueTypeValue;
        public object EditedValueTypeValue
        {
            get => _editedValueTypeValue;
            private set
            {
                if (_editedValueTypeValue == value)
                    return;

                _editedValueTypeValue = value;
                OnPropertyChanged();
            }
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public bool WrapValueTypeObject
        {
            get => (bool)GetValue(WrapValueTypeObjectProperty);
            set => SetValue(WrapValueTypeObjectProperty, value);
        }

        public IList<Type> NewItemTypeRegistry
        {
            get => (IList<Type>)GetValue(NewItemTypeRegistryProperty);
            set => SetValue(NewItemTypeRegistryProperty, value);
        }

        public bool CompactMode
        {
            get => (bool)GetValue(CompactModeProperty);
            set => SetValue(CompactModeProperty, value);
        }

        public bool ShowNavigationButtons
        {
            get => (bool)GetValue(ShowNavigationButtonsProperty);
            set => SetValue(ShowNavigationButtonsProperty, value);
        }

        public bool ShowHeader
        {
            get => (bool)GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        public double PopupsWidth
        {
            get => (double)GetValue(PopupsWidthProperty);
            set => SetValue(PopupsWidthProperty, value);
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

        private IIconDescriptor _iconDescriptor;
        public IIconDescriptor IconDescriptor
        {
            get => _iconDescriptor;
            private set
            {
                if (_iconDescriptor == value)
                    return;

                _iconDescriptor = value;
                OnPropertyChanged();
            }
        }

        private IIconDescriptor _systemIconDescriptor;
        public IIconDescriptor SystemIconDescriptor
        {
            get => _systemIconDescriptor;
            private set
            {
                if (_systemIconDescriptor == value)
                    return;

                _systemIconDescriptor = value;
                OnPropertyChanged();
            }
        }

        private IIconDescriptor _componentIconDescriptor;
        public IIconDescriptor ComponentIconDescriptor
        {
            get => _componentIconDescriptor;
            private set
            {
                if (_componentIconDescriptor == value)
                    return;

                _componentIconDescriptor = value;
                OnPropertyChanged();
            }
        }

        public string WorkingDirectory
        {
            get => (string)GetValue(WorkingDirectoryProperty);
            set => SetValue(WorkingDirectoryProperty, value);
        }

        public PropertyGridContentFileTypeResolver PropertyGridContentFileTypeResolver { get; } = new PropertyGridContentFileTypeResolver();

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

        public ICommand PreviousCommand
        {
            get => (ICommand)GetValue(PreviousCommandProperty);
            set => SetValue(PreviousCommandProperty, value);
        }

        public ICommand NextCommand
        {
            get => (ICommand)GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
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

        public IEnumerable<PropertyItemBase> Properties => PropertyGrid.Properties.Cast<PropertyItemBase>();

        static private readonly DependencyPropertyDescriptor EditorPropertyDescriptor;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyValueChangedEventHandler PropertyValueChanged;
        public event ItemEventHandler ItemSelected;

        static CalamePropertyGrid()
        {
            EditorPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(PropertyItemBase.EditorProperty, typeof(PropertyItem));
        }

        public CalamePropertyGrid()
        {
            InitializeComponent();
        }

        private void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (IsValueTypeObject)
            {
                if (WrapValueTypeObject)
                    EditedValueTypeValue = _valueTypeObject.Value;
                else
                    EditedValueTypeValue = SelectedObject;
            }

            PropertyValueChanged?.Invoke(this, e);
        }

        private void OnPreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            EditorPropertyDescriptor.AddValueChanged(e.PropertyItem, OnPropertyItemEditorChanged);
        }

        private void OnClearPropertyItem(object sender, PropertyItemEventArgs e)
        {
            EditorPropertyDescriptor.RemoveValueChanged(e.PropertyItem, OnPropertyItemEditorChanged);
        }

        private void OnPropertyItemEditorChanged(object sender, EventArgs e)
        {
            // Workaround to provide imported types and editor definition to collection controls
            if (((PropertyItem)sender).Editor is PropertyGridPopupOwnerBase propertyGridPopupOwnerBase)
            {
                propertyGridPopupOwnerBase.NewItemTypeRegistry = NewItemTypeRegistry;
                propertyGridPopupOwnerBase.EditorDefinitions = PropertyGrid.EditorDefinitions;
            }
        }

        static private void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (CalamePropertyGrid)d;

            Type selectedObjectType = propertyGrid.SelectedObject?.GetType();
            propertyGrid.IsValueTypeObject = selectedObjectType?.IsValueType ?? false;

            if (propertyGrid.WrapValueTypeObject && propertyGrid.IsValueTypeObject)
            {
                // Only instantiate a new ValueTypeObject when type is different to prevent property grid refresh and losing focus.
                if (propertyGrid._valueType != selectedObjectType)
                {
                    propertyGrid._valueType = selectedObjectType;
                    propertyGrid._valueTypeObject = (IValueTypeObject)Activator.CreateInstance(typeof(ValueTypeObject<>).MakeGenericType(selectedObjectType));
                }

                // Update displayed value-type value
                propertyGrid._valueTypeObject.Value = propertyGrid.SelectedObject;

                propertyGrid.DisplayedObject = propertyGrid._valueTypeObject;
            }
            else
            {
                propertyGrid._valueType = null;
                propertyGrid._valueTypeObject = null;

                propertyGrid.DisplayedObject = propertyGrid.SelectedObject;
            }
        }

        static private void OnIconDescriptorManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (CalamePropertyGrid)d;

            propertyGrid.IconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor();
            propertyGrid.SystemIconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor<CalameIconKey>();
            propertyGrid.ComponentIconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor<IGlyphComponent>();
        }

        static private void OnContentFileTypeResolverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (CalamePropertyGrid)d;

            propertyGrid.PropertyGridContentFileTypeResolver.DefaultResolver = propertyGrid.ContentFileTypeResolver;
        }

        static private void OnRawContentLibraryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (CalamePropertyGrid)d;

            propertyGrid.PropertyGridContentFileTypeResolver.RawContentLibrary = propertyGrid.RawContentLibrary;
        }

        private void OnItemSelected(object sender, ItemEventArgs args)
        {
            ItemSelected?.Invoke(this, args);
        }

        public interface IValueTypeObject
        {
            object Value { get; set; }
        }

        public class ValueTypeObject<T> : NotifyPropertyChangedBase, IValueTypeObject
        {
            private T _value;
            public T Value
            {
                get => _value;
                set => Set(ref _value, value);
            }

            object IValueTypeObject.Value
            {
                get => Value;
                set => Value = (T)value;
            }
        }
    }
}
