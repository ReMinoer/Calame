using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Calame.Utils;
using Diese.Collections;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;

namespace Calame.UserControls
{
    public interface ITreeContext
    {
        ITreeViewItemModel CreateTreeItemModel(object data);
    }

    public partial class CalameTreeView : UserControl, INotifyPropertyChanged
    {
        static public readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CalameTreeView), new PropertyMetadata(default(IEnumerable), OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        static private readonly Dictionary<CalameTreeView, ObservableListSynchronizer<object, ITreeViewItemModel>> Synchronizers
            = new Dictionary<CalameTreeView, ObservableListSynchronizer<object, ITreeViewItemModel>>();

        static private void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CalameTreeView)d;
            var itemsSource = (IEnumerable)e.NewValue;

            if (!Synchronizers.TryGetValue(control, out ObservableListSynchronizer<object, ITreeViewItemModel> synchronizer))
            {
                synchronizer = new ObservableListSynchronizer<object, ITreeViewItemModel>(x => control.TreeContext.CreateTreeItemModel(x));
                synchronizer.Subscribe(control._treeItems);
                Synchronizers.Add(control, synchronizer);
            }

            synchronizer.Reference = new EnumerableReadOnlyObservableList(itemsSource);
        }

        static public readonly DependencyProperty TreeContextProperty
            = DependencyProperty.Register(nameof(TreeContext), typeof(ITreeContext), typeof(CalameTreeView), new PropertyMetadata(default(ITreeContext)));

        public ITreeContext TreeContext
        {
            get => (ITreeContext)GetValue(TreeContextProperty);
            set => SetValue(TreeContextProperty, value);
        }

        static public readonly DependencyProperty SelectedItemProperty
            = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CalameTreeView), new PropertyMetadata(default(object), OnSelectedItemChanged));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        static private void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CalameTreeView)d;
            object value = e.NewValue;

            control.SelectedTreeItem = control.TreeItems.SelectMany(x => Tree.BreadthFirst(x, y => y.Children)).FirstOrDefault(y => y.Data == value);
        }

        private readonly ObservableList<ITreeViewItemModel> _treeItems;
        public IReadOnlyObservableList<ITreeViewItemModel> TreeItems { get; }
        
        private ITreeViewItemModel _selectedTreeItem;
        public ITreeViewItemModel SelectedTreeItem
        {
            get => _selectedTreeItem;
            set
            {
                if (_selectedTreeItem?.Equals(value) ?? value == null)
                    return;

                _selectedTreeItem = value;
                SelectedItem = _selectedTreeItem?.Data;

                NotifyPropertyChanged();
            }
        }
        
        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText == value)
                    return;

                _filterText = value;
                NotifyPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public CalameTreeView()
        {
            _treeItems = new ObservableList<ITreeViewItemModel>();
            TreeItems = new ReadOnlyObservableList<ITreeViewItemModel>(_treeItems);

            InitializeComponent();
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
