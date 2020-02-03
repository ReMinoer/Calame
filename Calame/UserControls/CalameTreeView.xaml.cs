using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Calame.Icons;
using Calame.Utils;
using Diese.Collections;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;

namespace Calame.UserControls
{
    public interface ITreeContext
    {
        ITreeViewItemModel CreateTreeItemModel(object data, Func<object, ITreeViewItemModel> dataConverter, Action<ITreeViewItemModel> itemDisposer);
        bool BaseFilter(object data);
    }

    public partial class CalameTreeView : UserControl, INotifyPropertyChanged
    {
        static public readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CalameTreeView), new PropertyMetadata(default(IEnumerable), OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        static private readonly Dictionary<CalameTreeView, ObservableListSynchronizer<object, ITreeViewItemModel>> Synchronizers = new Dictionary<CalameTreeView, ObservableListSynchronizer<object, ITreeViewItemModel>>();

        static private void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = (CalameTreeView)d;
            var itemsSource = (IEnumerable)e.NewValue;

            if (!Synchronizers.TryGetValue(treeView, out ObservableListSynchronizer<object, ITreeViewItemModel> synchronizer))
            {
                synchronizer = new ObservableListSynchronizer<object, ITreeViewItemModel>(x => SynchronizerConverter(treeView, x), x => x.Data, x => SynchronizerDisposer(treeView, x));
                synchronizer.Subscribe(treeView._treeItems);
                Synchronizers.Add(treeView, synchronizer);
            }
            
            synchronizer.Reference = itemsSource != null ? new EnumerableReadOnlyObservableList(itemsSource) : null;
            treeView.UpdateFilter();
        }

        static private ITreeViewItemModel SynchronizerConverter(CalameTreeView treeView, object data)
        {
            ITreeViewItemModel item = treeView.TreeContext.CreateTreeItemModel(data, x => SynchronizerConverter(treeView, x), x => SynchronizerDisposer(treeView, x));
            item.Children.CollectionChanged += treeView.OnItemChanged;
            return item;
        }

        static private void SynchronizerDisposer(CalameTreeView treeView, ITreeViewItemModel item)
        {
            item.Children.CollectionChanged -= treeView.OnItemChanged;
            item.Dispose();
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

        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(CalameTreeView), new PropertyMetadata());

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
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
                UpdateFilter();
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CalameTreeView()
        {
            _treeItems = new ObservableList<ITreeViewItemModel>();
            TreeItems = new ReadOnlyObservableList<ITreeViewItemModel>(_treeItems);

            _treeItems.CollectionChanged += OnItemChanged;

            InitializeComponent();
        }

        private void OnItemChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ITreeViewItemModel newItem in e.NewItems)
                        UpdateFilterOnItemAdded(newItem);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ITreeViewItemModel oldItem in e.OldItems)
                        UpdateFilterOnItemRemoved(oldItem);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (ITreeViewItemModel oldItem in e.OldItems)
                        UpdateFilterOnItemRemoved(oldItem);
                    foreach (ITreeViewItemModel newItem in e.NewItems)
                        UpdateFilterOnItemAdded(newItem);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (ITreeViewItemModel treeItem in TreeItems)
                        UpdateFilterOnItemRemoved(treeItem);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void UpdateFilter()
        {
            // If the selected tree item is filtered, it will be unselect
            // during the full filter refresh and change the content of an enumerable.
            // So we first unselect the item if necessary. 
            if (SelectedTreeItem != null)
            {
                UpdateFilter(SelectedTreeItem);
                if (!SelectedTreeItem.VisibleForFilter)
                    SelectedTreeItem = null;
            }

            foreach (ITreeViewItemModel treeItem in TreeItems)
                UpdateFilter(treeItem);
        }

        private void UpdateFilter(ITreeViewItemModel item)
        {
            foreach (ITreeViewItemModel child in item.Children)
                UpdateFilter(child);

            ApplyFilterOnItem(item);
        }

        private void UpdateFilterOnItemAdded(ITreeViewItemModel newItem)
        {
            // Update filter on new item and its children
            UpdateFilter(newItem);

            // Re-apply filter on parents
            foreach (ITreeViewItemModel parent in Sequence.AggregateExclusive(newItem, x => x.Parent))
                ApplyFilterOnItem(parent);
        }

        private void UpdateFilterOnItemRemoved(ITreeViewItemModel oldItem)
        {
            // Re-apply filter on parents
            foreach (ITreeViewItemModel parent in Sequence.AggregateExclusive(oldItem, x => x.Parent))
                ApplyFilterOnItem(parent);
        }

        private void ApplyFilterOnItem(ITreeViewItemModel item)
        {
            bool baseFilter = TreeContext.BaseFilter(item.Data);

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                item.MatchingFilter = false;
                item.VisibleForFilter = baseFilter || item.Children.Any(x => x.VisibleForFilter);
                item.VisibleAsParent = item.VisibleForFilter && !baseFilter;
                item.IsExpanded = item.VisibleAsParent || item.Children.Any(x => x.IsExpanded) || item.Children.Contains(SelectedTreeItem);
                return;
            }

            item.MatchingFilter = baseFilter && FilterByText(item);
            item.VisibleForFilter = item.MatchingFilter || item.Children.Any(x => x.VisibleForFilter);
            item.VisibleAsParent = item.VisibleForFilter && !item.MatchingFilter;
            item.IsExpanded = item.Children.Any(x => x.MatchingFilter) || item.Children.Any(x => x.IsExpanded);
        }

        private bool FilterByText(ITreeViewItemModel item)
        {
            return item.DisplayName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}