﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Calame.Icons;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public partial class CalamePropertyGrid : UserControl
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

        public IIconDescriptor ComponentIconDescriptor { get; private set; }

        public CalamePropertyGrid()
        {
            InitializeComponent();

            PropertyGrid.PreparePropertyItem += PropertyGridOnPreparePropertyItem;
            PropertyGrid.ClearPropertyItem += PropertyGridOnClearPropertyItem;
        }

        private void PropertyGridOnPreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            PropertyChangedEventManager.AddHandler(e.PropertyItem, PropertyItemOnEditorChanged, nameof(PropertyItem.Editor));
        }

        private void PropertyGridOnClearPropertyItem(object sender, PropertyItemEventArgs e)
        {
            PropertyChangedEventManager.RemoveHandler(e.PropertyItem, PropertyItemOnEditorChanged, nameof(PropertyItem.Editor));
        }

        private void PropertyItemOnEditorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (((PropertyItem)sender).Editor is InlineCollectionControl inlineCollectionControl)
            {
                inlineCollectionControl.NewItemTypeRegistry = NewItemTypeRegistry;
                inlineCollectionControl.EditorDefinitions = PropertyGrid.EditorDefinitions;
            }
        }

        static private void OnIconDescriptorManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (CalamePropertyGrid)d;
            propertyGrid.ComponentIconDescriptor = propertyGrid.IconDescriptorManager?.GetDescriptor<IGlyphComponent>();
        }

        public void Dispose()
        {
            PropertyGrid.PreparePropertyItem -= PropertyGridOnPreparePropertyItem;
            PropertyGrid.ClearPropertyItem -= PropertyGridOnClearPropertyItem;
        }

        public IIconTargetSelector GlyphCreatorIconTargetSelector { get; } = new GlyphCreatorIconTargetSelectorImplementation();
        private class GlyphCreatorIconTargetSelectorImplementation : IIconTargetSelector
        {
            public object GetIconTarget(object model) => ((IGlyphCreator)model).BindedObject;
        }
    }
}
