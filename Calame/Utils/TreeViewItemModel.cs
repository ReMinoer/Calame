using System;
using System.Collections.Generic;
using System.Linq;
using Calame.Icons;
using Diese;
using Diese.Collections;
using Diese.Collections.Children;
using Diese.Collections.Children.Observables;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;

namespace Calame.Utils
{
    public interface ITreeViewItemModel : IParentable<ITreeViewItemModel>, IEquatable<ITreeViewItemModel>, IDisposable
    {
        object Data { get; }
        string DisplayName { get; }
        IconDescription IconDescription { get; }

        new ITreeViewItemModel Parent { get; set; }
        IObservableList<ITreeViewItemModel> Children { get; }

        bool IsEnabled { get; set; }
        bool IsDisabledByParent { get; set; }
        
        bool IsExpanded { get; set; }
        bool IsTriggered { get; }

        bool MatchingBaseFilter { get; set; }
        bool MatchingUserFilter { get; set; }
        bool VisibleForFilter { get; set; }
        bool VisibleAsParent { get; set; }
    }

    public class TreeViewItemModel<T> : NotifyPropertyChangedBase, ITreeViewItemModel
    {
        protected readonly T Data;
        object ITreeViewItemModel.Data => Data;

        public IObservableList<ITreeViewItemModel> Children { get; }
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
            set => Set(ref _displayName, value);
        }

        private IconDescription _iconDescription;
        public IconDescription IconDescription
        {
            get => _iconDescription;
            set => Set(ref _iconDescription, value);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (!Set(ref _isEnabled, value))
                    return;

                foreach (ITreeViewItemModel item in Tree.DepthFirst<ITreeViewItemModel>(this, x => x.Children))
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
                
                _parent?.Children.Remove(this);
                _parent = value;
                _parent?.Children.Add(this);
            }
        }

        public TreeViewItemModel(T data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            Data = data;

            Children = new ObservableChildrenList<ITreeViewItemModel, ITreeViewItemModel>(this);
            ChildrenSynchronizer = new ObservableListSynchronizer<object, ITreeViewItemModel>(synchronizerConfiguration);
            ChildrenSynchronizer.Subscribe(Children);
        }

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