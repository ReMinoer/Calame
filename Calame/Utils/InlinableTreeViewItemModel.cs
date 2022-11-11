using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Calame.Icons;
using Diese;
using Diese.Collections.Observables.ReadOnly;

namespace Calame.Utils
{
    public class InlinableTreeViewItemModel<T> : NotifyPropertyChangedBase, ITreeViewItemModel
    {
        private readonly ITreeViewItemModel _baseItem;
        private readonly InlinableReadOnlyObservableList<ITreeViewItemModel> _inlinableChildren;

        private bool IsInlined => _inlinableChildren.IsInlined;
        private ITreeViewItemModel InlinedItem => _inlinableChildren.InlinedItem;
        private ITreeViewItemModel RedirectedItem => InlinedItem ?? _baseItem;
        
        public IReadOnlyObservableList<ITreeViewItemModel> Children => _inlinableChildren;
        public IReadOnlyObservableList<ITreeViewItemModel> LogicChildren => _baseItem.LogicChildren;

        static private readonly string[] RedirectedPropertiesNames =
        {
            nameof(QuickCommand),
            nameof(QuickCommandIconDescription),
            nameof(QuickCommandLabel),
            nameof(QuickCommandToolTip),
        };

        public InlinableTreeViewItemModel(ITreeViewItemModel baseItem, Func<ITreeViewItemModel, bool> canInlineChild)
        {
            _baseItem = baseItem;
            _inlinableChildren = new InlinableReadOnlyObservableList<ITreeViewItemModel>(_baseItem.Children, x => canInlineChild(x) ? x.Children : null);

            _baseItem.PropertyChanged += OnBaseItemPropertyChanged;
            _inlinableChildren.IsInlinedChanged += OnIsInlinedChanged;

            if (IsInlined)
            {
                InlinedItem.PropertyChanged += OnInlinedItemPropertyChanged;
            }
        }

        private void OnBaseItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsInlined && RedirectedPropertiesNames.Contains(e.PropertyName))
                return;

            NotifyPropertyChanged(e.PropertyName);
        }

        private void OnIsInlinedChanged(object sender, EventArgs e)
        {
            if (IsInlined)
            {
                InlinedItem.PropertyChanged += OnInlinedItemPropertyChanged;
            }
            else
            {
                if (InlinedItem != null)
                    InlinedItem.PropertyChanged -= OnInlinedItemPropertyChanged;
            }

            foreach (string redirectedPropertyName in RedirectedPropertiesNames)
            {
                NotifyPropertyChanged(redirectedPropertyName);
            }
        }

        private void OnInlinedItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (RedirectedPropertiesNames.Contains(e.PropertyName))
                NotifyPropertyChanged(e.PropertyName);
        }

        public ICommand QuickCommand => RedirectedItem.QuickCommand;
        public IconDescription QuickCommandIconDescription => RedirectedItem.QuickCommandIconDescription;
        public string QuickCommandLabel => RedirectedItem.QuickCommandLabel;
        public string QuickCommandToolTip => RedirectedItem.QuickCommandToolTip;

        public DraggedData GetDraggedData() => _baseItem.GetDraggedData();

        public void OnDragEnter(DragEventArgs eventArgs)
        {
            _baseItem.OnDragEnter(eventArgs);
            InlinedItem?.OnDragEnter(eventArgs);
        }

        public void OnDragOver(DragEventArgs eventArgs)
        {
            _baseItem.OnDragOver(eventArgs);
            InlinedItem?.OnDragOver(eventArgs);
        }

        public void OnDragLeave(DragEventArgs eventArgs)
        {
            _baseItem.OnDragLeave(eventArgs);
            InlinedItem?.OnDragLeave(eventArgs);
        }

        public void OnDrop(DragEventArgs eventArgs)
        {
            _baseItem.OnDrop(eventArgs);
            InlinedItem?.OnDrop(eventArgs);
        }

        public object Data => _baseItem.Data;
        public string DisplayName => _baseItem.DisplayName;
        public FontWeight FontWeight => _baseItem.FontWeight;
        public IconDescription IconDescription => _baseItem.IconDescription;
        public bool IsHeader => _baseItem.IsHeader;
        public bool IsTriggered => _baseItem.IsTriggered;
        public IEnumerable ContextMenuItems => _baseItem.ContextMenuItems;

        public ITreeViewItemModel Parent
        {
            get => _baseItem.Parent;
            set => _baseItem.Parent = value;
        }

        public bool IsEnabled
        {
            get => _baseItem.IsEnabled;
            set => _baseItem.IsEnabled = value;
        }

        public bool IsDisabledByParent
        {
            get => _baseItem.IsDisabledByParent;
            set => _baseItem.IsDisabledByParent = value;
        }

        public bool IsExpanded
        {
            get => _baseItem.IsExpanded;
            set => _baseItem.IsExpanded = value;
        }

        public bool MatchingBaseFilter
        {
            get => _baseItem.MatchingBaseFilter;
            set => _baseItem.MatchingBaseFilter = value;
        }

        public bool MatchingUserFilter
        {
            get => _baseItem.MatchingUserFilter;
            set => _baseItem.MatchingUserFilter = value;
        }

        public bool VisibleForFilter
        {
            get => _baseItem.VisibleForFilter;
            set => _baseItem.VisibleForFilter = value;
        }

        public bool VisibleAsParent
        {
            get => _baseItem.VisibleAsParent;
            set => _baseItem.VisibleAsParent = value;
        }

        public void AddChild(ITreeViewItemModel child) => _baseItem.AddChild(child);
        public void RemoveChild(ITreeViewItemModel child) => _baseItem.RemoveChild(child);

        public bool Equals(ITreeViewItemModel other) => _baseItem.Equals(other);
        public void Dispose() { _baseItem.Dispose(); }
    }
}