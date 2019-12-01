using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Diese.Collections;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Controls
{
    public partial class InlineCollectionControl : UserControl
    {
        static public readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IList), typeof(InlineCollectionControl), new PropertyMetadata(null));
        static public readonly DependencyProperty NewItemTypeRegistryProperty =
            DependencyProperty.Register(nameof(NewItemTypeRegistry), typeof(IList<Type>), typeof(InlineCollectionControl), new PropertyMetadata(null));
        static public readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(InlineCollectionControl), new PropertyMetadata(false));
        static public readonly DependencyProperty EditorDefinitionsProperty =
            DependencyProperty.Register(nameof(EditorDefinitions), typeof(EditorDefinitionCollection), typeof(InlineCollectionControl), new PropertyMetadata(null));

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IList<Type> NewItemTypeRegistry
        {
            get => (IList<Type>)GetValue(NewItemTypeRegistryProperty);
            set => SetValue(NewItemTypeRegistryProperty, value);
        }
        
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public EditorDefinitionCollection EditorDefinitions
        {
            get => (EditorDefinitionCollection)GetValue(EditorDefinitionsProperty);
            set => SetValue(EditorDefinitionsProperty, value);
        }

        public InlineCollectionControl()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var collectionControl = new CollectionControl
            {
                Margin = new Thickness(10)
            };

            Type[] interfaces = ItemsSource.GetType().GetInterfaces();
            if (interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>), out Type collectionType))
            {
                Type itemType = collectionType.GenericTypeArguments[0];
                collectionControl.NewItemTypes = NewItemTypeRegistry?.Where(x => itemType.IsAssignableFrom(x)).ToList();
            }
            
            collectionControl.ItemsSource = ItemsSource;

            var window = new Window
            {
                Width = 800,
                Height = 500,
                Content = collectionControl
            };
            
            collectionControl.PropertyGrid.EditorDefinitions = EditorDefinitions;

            collectionControl.ItemAdded += CollectionControlOnItemAdded;
            collectionControl.ItemDeleted += CollectionControlOnItemDeleted;
            collectionControl.ItemMovedUp += CollectionControlOnItemMovedUp;
            collectionControl.ItemMovedDown += CollectionControlOnItemMovedDown;

            window.ShowDialog();

            collectionControl.ItemAdded -= CollectionControlOnItemAdded;
            collectionControl.ItemDeleted -= CollectionControlOnItemDeleted;
            collectionControl.ItemMovedUp -= CollectionControlOnItemMovedUp;
            collectionControl.ItemMovedDown -= CollectionControlOnItemMovedDown;
        }

        private void CollectionControlOnItemAdded(object sender, ItemEventArgs itemEventArgs)
        {
            var collectionControl = (CollectionControl)sender;
            collectionControl.ItemsSource.Add(itemEventArgs.Item);
        }

        private void CollectionControlOnItemDeleted(object sender, ItemEventArgs itemEventArgs)
        {
            var collectionControl = (CollectionControl)sender;
            collectionControl.ItemsSource.Remove(itemEventArgs.Item);
        }

        private void CollectionControlOnItemMovedUp(object sender, ItemEventArgs itemEventArgs)
        {
            var collectionControl = (CollectionControl)sender;

            int index = collectionControl.ItemsSource.IndexOf(itemEventArgs.Item);
            collectionControl.ItemsSource.RemoveAt(index);
            collectionControl.ItemsSource.Insert(--index, itemEventArgs.Item);
        }

        private void CollectionControlOnItemMovedDown(object sender, ItemEventArgs itemEventArgs)
        {
            var collectionControl = (CollectionControl)sender;

            int index = collectionControl.ItemsSource.IndexOf(itemEventArgs.Item);
            collectionControl.ItemsSource.RemoveAt(index);
            collectionControl.ItemsSource.Insert(++index, itemEventArgs.Item);
        }
    }
}
