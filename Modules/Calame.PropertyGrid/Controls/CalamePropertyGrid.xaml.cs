using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Calame.Utils;
using Diese.Collections;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public partial class CalamePropertyGrid : UserControl
    {
        public IList<Type> NewItemTypeRegistry
        {
            get => (IList<Type>)GetValue(NewItemTypeRegistryProperty);
            set => SetValue(NewItemTypeRegistryProperty, value);
        }
        
        static public readonly DependencyProperty NewItemTypeRegistryProperty =
            DependencyProperty.Register("NewItemTypeRegistry", typeof(IList<Type>), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        
        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }
        
        static public readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(CalamePropertyGrid), new PropertyMetadata(null));
        
        public bool CompactMode
        {
            get => (bool)GetValue(CompactModeProperty);
            set => SetValue(CompactModeProperty, value);
        }
        
        static public readonly DependencyProperty CompactModeProperty =
            DependencyProperty.Register("CompactMode", typeof(bool), typeof(CalamePropertyGrid), new PropertyMetadata(false));

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

        public void Dispose()
        {
            PropertyGrid.PreparePropertyItem -= PropertyGridOnPreparePropertyItem;
            PropertyGrid.ClearPropertyItem -= PropertyGridOnClearPropertyItem;
        }
    }
}
