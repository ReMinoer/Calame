using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Calame.Icons;
using Calame.UserControls;
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
        bool IsEnabled { get; set; }
        bool IsDisabledByParent { get; set; }
        bool IsExpanded { get; set; }
        bool MatchingFilter { get; set; }
        bool VisibleForFilter { get; set; }
        bool VisibleAsParent { get; set; }
        new ITreeViewItemModel Parent { get; set; }
        IObservableList<ITreeViewItemModel> Children { get; }
    }

    public class TreeViewItemModel<T> : NotifyPropertyChangedBase, ITreeViewItemModel
    {
        public T Data { get; }
        object ITreeViewItemModel.Data => Data;
        
        private readonly string _displayNamePropertyName;
        private readonly string _childrenPropertyName;
        private readonly INotifyPropertyChanged _displayNameNotifier;
        private readonly INotifyPropertyChanged _childrenNotifier;

        private readonly Func<T, string> _displayNameFunc;
        private readonly Func<T, IReadOnlyObservableList<object>> _childrenFunc;

        public IObservableList<ITreeViewItemModel> Children { get; }
        private readonly ObservableListSynchronizer<object, ITreeViewItemModel> _childrenSynchronizer;

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            private set => Set(ref _displayName, value);
        }

        public IconDescription IconDescription { get; }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (!Set(ref _isEnabled, value))
                    return;

                foreach (ITreeViewItemModel item in Tree.DepthFirst<ITreeViewItemModel, ITreeViewItemModel>(this, x => x.Children))
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

        private bool _matchingFilter;
        public bool MatchingFilter
        {
            get => _matchingFilter;
            set => Set(ref _matchingFilter, value);
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

        public TreeViewItemModel(
            ITreeContext treeContext,
            T data,
            Func<T, string> displayNameFunc,
            Func<T, IReadOnlyObservableList<object>> childrenFunc,
            IconDescription iconDescription,
            string displayNamePropertyName,
            string childrenPropertyName,
            INotifyPropertyChanged displayNameNotifier = null,
            INotifyPropertyChanged childrenNotifier = null)
        {
            Data = data;
            INotifyPropertyChanged notifyingData = null;
            
            _displayNameFunc = displayNameFunc;
            _displayNamePropertyName = displayNamePropertyName;
            _displayNameNotifier = displayNameNotifier ?? (notifyingData = Data as INotifyPropertyChanged);

            DisplayName = _displayNameFunc(Data);

            if (_displayNameNotifier != null)
                _displayNameNotifier.PropertyChanged += OnDisplayNameNotifierPropertyChanged;
            
            _childrenFunc = childrenFunc;
            _childrenPropertyName = childrenPropertyName;
            _childrenNotifier = childrenNotifier ?? notifyingData ?? Data as INotifyPropertyChanged;

            Children = new ObservableChildrenList<ITreeViewItemModel, ITreeViewItemModel>(this);

            _childrenSynchronizer = new ObservableListSynchronizer<object, ITreeViewItemModel>(_childrenFunc(Data), treeContext.CreateTreeItemModel, x => x.Data, x => x.Dispose());
            _childrenSynchronizer.Subscribe(Children);

            if (_childrenNotifier != null)
                _childrenNotifier.PropertyChanged += OnChildrenNotifierPropertyChanged;

            IconDescription = iconDescription;
        }

        private void OnDisplayNameNotifierPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _displayNamePropertyName)
                DisplayName = _displayNameFunc(Data);  
        }

        private void OnChildrenNotifierPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _childrenPropertyName)
                _childrenSynchronizer.Reference = _childrenFunc(Data);
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

        public void Dispose()
        {
            _displayNameNotifier.PropertyChanged -= OnDisplayNameNotifierPropertyChanged;
            _childrenNotifier.PropertyChanged -= OnChildrenNotifierPropertyChanged;
            _childrenSynchronizer.Dispose();

            foreach (ITreeViewItemModel child in Children)
                child.Dispose();
        }
    }
}