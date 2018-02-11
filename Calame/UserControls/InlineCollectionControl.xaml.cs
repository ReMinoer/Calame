using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Diese.Collections;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.UserControls
{
    public partial class InlineCollectionControl : UserControl
    {
        static public readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(InlineCollectionControl), new PropertyMetadata(null));
        static public readonly DependencyProperty NewItemTypesProperty =
            DependencyProperty.Register("NewItemTypes", typeof(IList<Type>), typeof(InlineCollectionControl), new PropertyMetadata(null));
        static public readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(InlineCollectionControl), new PropertyMetadata(false));

        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IList<Type> NewItemTypes
        {
            get => (IList<Type>)GetValue(NewItemTypesProperty);
            set => SetValue(NewItemTypesProperty, value);
        }
        
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        
        public InlineCollectionControl()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var collectionControl = new CollectionControl
            {
                Margin = new Thickness(10),
                NewItemTypes = new List<Type>()
            };

            if (ItemsSource.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>), out Type type))
            {
                Type itemType = type.GenericTypeArguments[0];
                collectionControl.NewItemTypes = NewItemTypes?.Where(x => itemType.IsAssignableFrom(x)).ToList();
            }

            collectionControl.ItemAdded += CollectionControlOnItemAdded;
            collectionControl.ItemDeleted += CollectionControlOnItemDeleted;
            collectionControl.ItemMovedUp += CollectionControlOnItemMovedUp;
            collectionControl.ItemMovedDown += CollectionControlOnItemMovedDown;
            
            BindingOperations.SetBinding(collectionControl, CollectionControl.ItemsSourceProperty, new Binding(nameof(ItemsSource))
            {
                Source = this,
                Mode = BindingOperations.GetBinding(this, ItemsSourceProperty)?.Mode ?? BindingMode.Default
            });
            
            BindingOperations.SetBinding(collectionControl, CollectionControl.IsReadOnlyProperty, new Binding(nameof(IsReadOnly))
            {
                Source = this,
                Mode = BindingOperations.GetBinding(this, IsReadOnlyProperty)?.Mode ?? BindingMode.Default
            });

            var window = new Window
            {
                Width = 800,
                Height = 500,
                Content = collectionControl
            };

            var resourceDictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/Calame;component/Templates/CalameEditorDefinitions.xaml") };
            collectionControl.PropertyGrid.Resources.MergedDictionaries.Add(resourceDictionary);

            if (collectionControl.PropertyGrid.TryFindResource("CalameEditorDefinitions") is EditorDefinitionCollection editorDefinitions)
                collectionControl.PropertyGrid.EditorDefinitions = editorDefinitions;

            window.ShowDialog();

            BindingOperations.ClearAllBindings(collectionControl);

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
