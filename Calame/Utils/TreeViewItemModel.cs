using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Calame.Icons;
using Diese;
using Diese.Collections;
using Diese.Collections.Children;
using Diese.Collections.Children.Observables;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework;
using Glyph.Tools.UndoRedo;

namespace Calame.Utils
{
    public interface ITreeViewItemModel : IParentable<ITreeViewItemModel>, IEquatable<ITreeViewItemModel>, IDragSource, IDropTarget, INotifyPropertyChanged, IDisposable
    {
        object Data { get; }

        string DisplayName { get; set; }
        string EditableDisplayName { get; set; }
        bool CanEditDisplayName { get; }
        ICommand EditDisplayNameCommand { get; }
        bool IsEditingDisplayName { get; set; }

        FontWeight FontWeight { get; }
        IconDescription IconDescription { get; }

        new ITreeViewItemModel Parent { get; set; }
        IReadOnlyObservableList<ITreeViewItemModel> Children { get; }
        IReadOnlyObservableList<ITreeViewItemModel> LogicChildren { get; }
        void AddChild(ITreeViewItemModel child);
        void RemoveChild(ITreeViewItemModel child);

        bool IsHeader { get; }
        bool IsEnabled { get; set; }
        bool IsDisabledByParent { get; set; }
        
        bool IsExpanded { get; set; }
        bool IsTriggered { get; }

        bool MatchingBaseFilter { get; set; }
        bool MatchingUserFilter { get; set; }
        bool VisibleForFilter { get; set; }
        bool VisibleAsParent { get; set; }

        IEnumerable ContextMenuItems { get; }
        ICommand QuickCommand { get; }
        IconDescription QuickCommandIconDescription { get; }
        string QuickCommandLabel { get; }
        string QuickCommandToolTip { get; }
    }

    public class TreeViewItemModel<T> : NotifyPropertyChangedBase, ITreeViewItemModel
    {
        private readonly IUndoRedoStack _undoRedoStack;

        protected readonly T Data;
        object ITreeViewItemModel.Data => Data;
        
        private readonly ObservableChildrenList<ITreeViewItemModel, ITreeViewItemModel> _children;
        public IReadOnlyObservableList<ITreeViewItemModel> Children { get; }
        public IReadOnlyObservableList<ITreeViewItemModel> LogicChildren => Children;
        protected readonly ObservableListSynchronizer<object, ITreeViewItemModel> ChildrenSynchronizer;
        
        public IReadOnlyObservableList<object> ChildrenSource
        {
            get => ChildrenSynchronizer.Reference;
            set => ChildrenSynchronizer.Reference = value;
        }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (Set(ref _displayName, value))
                {
                    DisplayNameSetter?.Invoke(value);
                    NotifyPropertyChanged(nameof(EditableDisplayName));
                }
            }
        }
        
        public string EditableDisplayName
        {
            get => DisplayName;
            set
            {
                if (DisplayName == value)
                    return;

                string oldValue = DisplayName;

                _undoRedoStack.Execute($"Set item name to {value}",
                    () => DisplayName = value,
                    () => DisplayName = oldValue);
            }
        }

        private bool _canEditDisplayNam;
        public bool CanEditDisplayName
        {
            get => _canEditDisplayNam;
            set => Set(ref _canEditDisplayNam, value);
        }

        private bool _isEditingDisplayName;
        public bool IsEditingDisplayName
        {
            get => _isEditingDisplayName;
            set => Set(ref _isEditingDisplayName, value);
        }

        private Action<string> _displayNameSetter;
        public Action<string> DisplayNameSetter
        {
            get => _displayNameSetter;
            set => Set(ref _displayNameSetter, value);
        }

        private FontWeight _fontWeight;
        public FontWeight FontWeight
        {
            get => _fontWeight;
            set => Set(ref _fontWeight, value);
        }

        private IconDescription _iconDescription;
        public IconDescription IconDescription
        {
            get => _iconDescription;
            set => Set(ref _iconDescription, value);
        }

        private bool _isHeader;
        public bool IsHeader
        {
            get => _isHeader;
            set => Set(ref _isHeader, value);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (!Set(ref _isEnabled, value))
                    return;

                foreach (ITreeViewItemModel item in Tree.DepthFirst<ITreeViewItemModel>(this, x => x.LogicChildren))
                    item.IsDisabledByParent = Sequence.Aggregate(item, x => x.Parent).Any(x => !x.IsEnabled);
            }
        }

        private bool _isDisabledByParent;
        public bool IsDisabledByParent
        {
            get => _isDisabledByParent;
            set => Set(ref _isDisabledByParent, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        private bool _isTriggered;
        public bool IsTriggered
        {
            get => _isTriggered;
            set => Set(ref _isTriggered, value);
        }

        private bool _matchingBaseFilter;
        public bool MatchingBaseFilter
        {
            get => _matchingBaseFilter;
            set => Set(ref _matchingBaseFilter, value);
        }

        private bool _matchingUserFilter;
        public bool MatchingUserFilter
        {
            get => _matchingUserFilter;
            set => Set(ref _matchingUserFilter, value);
        }
        
        private bool _visibleForFilter = true;
        public bool VisibleForFilter
        {
            get => _visibleForFilter;
            set => Set(ref _visibleForFilter, value);
        }

        private bool _visibleAsParent;
        public bool VisibleAsParent
        {
            get => _visibleAsParent;
            set => Set(ref _visibleAsParent, value);
        }

        private ITreeViewItemModel _parent;
        public ITreeViewItemModel Parent
        {
            get => _parent;
            set
            {
                if (_parent.NullableEquals(value))
                    return;
                
                _parent?.RemoveChild(this);
                _parent = value;
                _parent?.AddChild(this);
            }
        }

        private IEnumerable _contextMenuItems;
        public IEnumerable ContextMenuItems
        {
            get => _contextMenuItems;
            set => Set(ref _contextMenuItems, value);
        }

        private ICommand _quickCommand;
        public ICommand QuickCommand
        {
            get => _quickCommand;
            set => Set(ref _quickCommand, value);
        }

        private IconDescription _quickCommandIconDescription;
        public IconDescription QuickCommandIconDescription
        {
            get => _quickCommandIconDescription;
            set => Set(ref _quickCommandIconDescription, value);
        }

        private string _quickCommandToolTip;
        public string QuickCommandToolTip
        {
            get => _quickCommandToolTip;
            set => Set(ref _quickCommandToolTip, value);
        }

        private string _quickCommandLabel;
        public string QuickCommandLabel
        {
            get => _quickCommandLabel;
            set => Set(ref _quickCommandLabel, value);
        }

        private Func<DraggedData> _draggedDataProvider;
        public Func<DraggedData> DraggedDataProvider
        {
            get => _draggedDataProvider;
            set => Set(ref _draggedDataProvider, value);
        }

        private Action<DragEventArgs> _dragEnterAction;
        public Action<DragEventArgs> DragEnterAction
        {
            get => _dragEnterAction;
            set => Set(ref _dragEnterAction, value);
        }

        private Action<DragEventArgs> _dragOverAction;
        public Action<DragEventArgs> DragOverAction
        {
            get => _dragOverAction;
            set => Set(ref _dragOverAction, value);
        }

        private Action<DragEventArgs> _dragLeaveAction;
        public Action<DragEventArgs> DragLeaveAction
        {
            get => _dragLeaveAction;
            set => Set(ref _dragLeaveAction, value);
        }

        private Action<DragEventArgs> _dropAction;
        public Action<DragEventArgs> DropAction
        {
            get => _dropAction;
            set => Set(ref _dropAction, value);
        }

        public ICommand EditDisplayNameCommand { get; }

        public TreeViewItemModel(T data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration, IUndoRedoStack undoRedoStack)
        {
            Data = data;
            _undoRedoStack = undoRedoStack;

            _children = new ObservableChildrenList<ITreeViewItemModel, ITreeViewItemModel>(this);
            Children = new ReadOnlyObservableList<ITreeViewItemModel>(_children);

            ChildrenSynchronizer = new ObservableListSynchronizer<object, ITreeViewItemModel>(synchronizerConfiguration);
            ChildrenSynchronizer.Subscribe(_children);

            EditDisplayNameCommand = new RelayCommand(_ => IsEditingDisplayName = true, _ => CanEditDisplayName);
        }

        public void AddChild(ITreeViewItemModel child) => _children.Add(child);
        public void RemoveChild(ITreeViewItemModel child) => _children.Remove(child);

        public DraggedData GetDraggedData() => DraggedDataProvider?.Invoke();
        public void OnDragEnter(DragEventArgs eventArgs) => DragEnterAction?.Invoke(eventArgs);
        public void OnDragOver(DragEventArgs eventArgs) => DragOverAction?.Invoke(eventArgs);
        public void OnDragLeave(DragEventArgs eventArgs) => DragLeaveAction?.Invoke(eventArgs);
        public void OnDrop(DragEventArgs eventArgs) => DropAction?.Invoke(eventArgs);

        public virtual void Dispose()
        {
            ChildrenSynchronizer.Dispose();
            foreach (ITreeViewItemModel child in Children)
                child.Dispose();
        }

        public bool Equals(ITreeViewItemModel other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (!(other.Data is T otherData))
                return false;

            return EqualityComparer<T>.Default.Equals(Data, otherData);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!(obj is ITreeViewItemModel other))
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Data);
        }
    }
}