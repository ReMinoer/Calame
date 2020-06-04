using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Calame.Icons;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public partial class CalamePropertyGrid : INotifyPropertyChanged
    {
        static public readonly DependencyProperty NewItemTypeRegistryProperty =
            DependencyProperty.Register(nameof(NewItemTypeRegistry), typeof(IList<Type>), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        static public readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        static public readonly DependencyProperty CompactModeProperty =
            DependencyProperty.Register(nameof(CompactMode), typeof(bool), typeof(CalamePropertyGrid), new PropertyMetadata(false));
        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        static public readonly DependencyProperty IconDescriptorManagerProperty =
            DependencyProperty.Register(nameof(IconDescriptorManager), typeof(IIconDescriptorManager), typeof(CalamePropertyGrid), new PropertyMetadata(null, OnIconDescriptorManagerChanged));
        
        public IList<Type> NewItemTypeRegistry
        {
            get => (IList<Type>)GetValue(NewItemTypeRegistryProperty);
            set => SetValue(NewItemTypeRegistryProperty, value);
        }
        
        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }
        
        public bool CompactMode
        {
            get => (bool)GetValue(CompactModeProperty);
            set => SetValue(CompactModeProperty, value);
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

        static private readonly DependencyPropertyDescriptor EditorPropertyDescriptor;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyValueChangedEventHandler PropertyValueChanged;
        public event ItemEventHandler ShowItemInPropertyGrid;

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
            if (((PropertyItem)sender).Editor is InlineCollectionControl inlineCollectionControl)
            {
                inlineCollectionControl.NewItemTypeRegistry = NewItemTypeRegistry;
                inlineCollectionControl.EditorDefinitions = PropertyGrid.EditorDefinitions;
            }
        }

        static private void OnIconDescriptorManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (CalamePropertyGrid)d;

            propertyGrid.IconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor();
            propertyGrid.SystemIconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor<CalameIconKey>();
            propertyGrid.ComponentIconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor<IGlyphComponent>();
        }

        private void OnShowItemInPropertyGrid(object sender, ItemEventArgs args)
        {
            ShowItemInPropertyGrid?.Invoke(this, args);
        }

        public IIconTargetSelector GlyphCreatorIconTargetSelector { get; } = new GlyphCreatorIconTargetSelectorImplementation();
        private class GlyphCreatorIconTargetSelectorImplementation : IIconTargetSelector
        {
            public object GetIconTarget(object model) => ((IGlyphCreator)model).BindedObject;
        }
    }
}
