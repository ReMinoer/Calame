using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Calame.Icons;
using Calame.Utils;
using Diese.Collections;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework;

namespace Calame.UserControls
{
    public interface ITreeContext
    {
        bool DisableChildrenIfParentDisabled { get; }
        event EventHandler BaseFilterChanged;
        ITreeViewItemModel CreateTreeItemModel(object data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration);
        bool IsMatchingBaseFilter(object data);
    }

    public partial class CalameTreeView : INotifyPropertyChanged, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel>
    {
        static public readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CalameTreeView), new PropertyMetadata(default(IEnumerable), OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        static private void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = (CalameTreeView)d;
            var itemsSource = (IEnumerable)e.NewValue;

            object currentlySelectedItem = treeView.SelectedItem;
            treeView.SelectedTreeItem = null;

            treeView._synchronizer.Reference = itemsSource != null ? new EnumerableReadOnlyObservableList(itemsSource) : null;
            treeView.UpdateFilter(forceExpandOrCollapse: true);

            if (currentlySelectedItem != null && treeView.TreeItems.SelectMany(x => Tree.BreadthFirst(x, y => y.Children)).Any(y => y.Data == currentlySelectedItem))
                treeView.SelectedItem = currentlySelectedItem;
        }

        ITreeViewItemModel ICollectionSynchronizerConfiguration<object, ITreeViewItemModel>.CreateItem(object referenceItem)
            => TreeContext.CreateTreeItemModel(referenceItem, this);
        object ICollectionSynchronizerConfiguration<object, ITreeViewItemModel>.GetReference(ITreeViewItemModel collectedItem)
            => collectedItem.Data;
        void ICollectionSynchronizerConfiguration<object, ITreeViewItemModel>.SubscribeItem(ITreeViewItemModel collectedItem)
            => collectedItem.LogicChildren.CollectionChanged += OnItemChanged;
        void ICollectionSynchronizerConfiguration<object, ITreeViewItemModel>.UnsubscribeItem(ITreeViewItemModel collectedItem)
            => collectedItem.LogicChildren.CollectionChanged -= OnItemChanged;
        void ICollectionSynchronizerConfiguration<object, ITreeViewItemModel>.DisposeItem(ITreeViewItemModel collectedItem)
            => collectedItem.Dispose();

        static public readonly DependencyProperty TreeContextProperty
            = DependencyProperty.Register(nameof(TreeContext), typeof(ITreeContext), typeof(CalameTreeView), new PropertyMetadata(default(ITreeContext), OnTreeContextChanged));

        public ITreeContext TreeContext
        {
            get => (ITreeContext)GetValue(TreeContextProperty);
            set => SetValue(TreeContextProperty, value);
        }

        static private void OnTreeContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = (CalameTreeView)d;
            var oldValue = (ITreeContext)e.OldValue;
            var newValue = (ITreeContext)e.NewValue;

            object currentlySelectedItem = treeView.SelectedItem;

            if (oldValue != null)
            {
                oldValue.BaseFilterChanged -= treeView.OnBaseFilterChanged;

                treeView.SelectedItem = null;
                treeView._synchronizer.Unsubscribe(treeView._treeItems);
            }

            if (newValue != null)
            {
                treeView._synchronizer.Subscribe(treeView._treeItems);
                treeView.SelectedItem = currentlySelectedItem;

                treeView.UpdateFilter(forceExpandOrCollapse: true);
                newValue.BaseFilterChanged += treeView.OnBaseFilterChanged;
            }
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

            if (value == null)
            {
                control.SelectedTreeItem = null;
                return;
            }
            
            control.SelectedTreeItem = control.TreeItems.SelectMany(x => Tree.BreadthFirst(x, y => y.Children)).FirstOrDefault(y => y.Data == value);
        }

        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(CalameTreeView), new PropertyMetadata());

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        static public readonly DependencyProperty IconDescriptorProperty =
            DependencyProperty.Register(nameof(IconDescriptor), typeof(IIconDescriptor), typeof(CalameTreeView), new PropertyMetadata());

        public IIconDescriptor IconDescriptor
        {
            get => (IIconDescriptor)GetValue(IconDescriptorProperty);
            set => SetValue(IconDescriptorProperty, value);
        }
        
        private readonly ObservableListSynchronizer<object, ITreeViewItemModel> _synchronizer;
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

                if (string.IsNullOrEmpty(_filterText))
                    SelectedItem = null;

                _filterText = value;
                UpdateFilter(forceExpandOrCollapse: true);
                NotifyPropertyChanged();
            }
        }

        public ICommand CollapseAllCommand { get; }
        public ICommand ExpandAllCommand { get; }
        public ICommand FocusSelectionCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public CalameTreeView()
        {
            _treeItems = new ObservableList<ITreeViewItemModel>();
            TreeItems = new ReadOnlyObservableList<ITreeViewItemModel>(_treeItems);

            _synchronizer = new ObservableListSynchronizer<object, ITreeViewItemModel>(this);

            CollapseAllCommand = new RelayCommand(OnCollapseAll);
            ExpandAllCommand = new RelayCommand(OnExpandAll);
            FocusSelectionCommand = new RelayCommand(OnFocusSelection, CanFocusSelection);

            _treeItems.CollectionChanged += OnItemChanged;

            InitializeComponent();
        }

        private void OnCollapseAll(object _)
        {
            SelectedItem = null;

            foreach (ITreeViewItemModel treeItem in TreeItems)
                UpdateAllExpand(treeItem, false);
        }

        private void OnExpandAll(object _)
        {
            foreach (ITreeViewItemModel treeItem in TreeItems)
                UpdateAllExpand(treeItem, true);
        }

        private bool CanFocusSelection(object _) => SelectedTreeItem != null;
        private void OnFocusSelection(object _)
        {
            object selectedItem = SelectedItem;

            FilterText = null;
            OnCollapseAll(_);

            SelectedItem = selectedItem;
            FocusTreeView();
        }

        public bool IsTreeViewFocused { get; private set; }

        public void FocusTreeView()
        {
            IsTreeViewFocused = false;
            NotifyPropertyChanged(nameof(IsTreeViewFocused));
            IsTreeViewFocused = true;
            NotifyPropertyChanged(nameof(IsTreeViewFocused));
            IsTreeViewFocused = false;
            NotifyPropertyChanged(nameof(IsTreeViewFocused));
        }

        private void UpdateAllExpand(ITreeViewItemModel treeItem, bool value)
        {
            treeItem.IsExpanded = value;

            foreach (ITreeViewItemModel child in treeItem.LogicChildren)
                UpdateAllExpand(child, value);
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

        private void OnBaseFilterChanged(object sender, EventArgs eventArgs)
        {
            UpdateFilter(forceExpandOrCollapse: false);
        }

        private void UpdateFilter(bool forceExpandOrCollapse)
        {
            // If the selected tree item is filtered, it will be unselect
            // during the full filter refresh and change the content of an enumerable.
            // So we first unselect the item if necessary. 
            // TODO: Try to keep selection
            if (SelectedTreeItem != null)
            {
                UpdateFilter(SelectedTreeItem, forceExpandOrCollapse);
                if (!SelectedTreeItem.VisibleForFilter)
                    SelectedTreeItem = null;
            }

            foreach (ITreeViewItemModel treeItem in TreeItems)
                UpdateFilter(treeItem, forceExpandOrCollapse);
        }

        private void UpdateFilter(ITreeViewItemModel item, bool forceExpand)
        {
            foreach (ITreeViewItemModel child in item.LogicChildren)
                UpdateFilter(child, forceExpand);

            ApplyFilterOnItem(item, forceExpand);
        }

        private void UpdateFilterOnItemAdded(ITreeViewItemModel newItem)
        {
            // Update filter on new item and its children
            UpdateFilter(newItem, forceExpand: false);

            // Re-apply filter on parents
            foreach (ITreeViewItemModel parent in Sequence.AggregateExclusive(newItem, x => x.Parent))
                ApplyFilterOnItem(parent, forceExpandOrCollapse: false);
        }

        private void UpdateFilterOnItemRemoved(ITreeViewItemModel oldItem)
        {
            // Re-apply filter on parents
            foreach (ITreeViewItemModel parent in Sequence.AggregateExclusive(oldItem, x => x.Parent))
                ApplyFilterOnItem(parent, forceExpandOrCollapse: false);
        }

        private void ApplyFilterOnItem(ITreeViewItemModel item, bool forceExpandOrCollapse)
        {
            item.MatchingBaseFilter = TreeContext.IsMatchingBaseFilter(item.Data);
            item.MatchingUserFilter = item.MatchingBaseFilter && IsMatchingUserFilter(item);

            bool isMatchingActiveFilter = IsFilteredByUser
                ? item.MatchingUserFilter
                : item.MatchingBaseFilter;

            bool hasChildVisibleForFilter = item.LogicChildren.Any(x => x.VisibleForFilter);

            item.VisibleForFilter = isMatchingActiveFilter || hasChildVisibleForFilter;
            item.VisibleAsParent = !isMatchingActiveFilter && hasChildVisibleForFilter;
            
            if (forceExpandOrCollapse)
            {
                item.IsExpanded = IsFilteredByUser && hasChildVisibleForFilter;
            }
            else
            {
                if (IsFilteredByUser && hasChildVisibleForFilter)
                    item.IsExpanded = true;
            }
        }

        private bool IsFilteredByUser => !string.IsNullOrWhiteSpace(FilterText);
        private bool IsMatchingUserFilter(ITreeViewItemModel item)
        {
            return !string.IsNullOrWhiteSpace(FilterText)
                && item.DisplayName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}