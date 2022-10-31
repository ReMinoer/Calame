using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Calame.Icons;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Simulacra.Binding;

namespace Calame.Utils
{
    public class TreeViewItemModelBuilder<T>
    {
        private readonly List<Func<T, IBindingModule<TreeViewItemModel>>> _bindingModulesFunc = new List<Func<T, IBindingModule<TreeViewItemModel>>>();

        public TreeViewItemModelBuilder<T> DisplayName(Func<T, string> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => DisplayName(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> FontWeight(Func<T, FontWeight> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => FontWeight(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> IconDescription(Func<T, IconDescription> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => IconDescription(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> ChildrenSource(Func<T, IReadOnlyObservableList<object>> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => ChildrenSource(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> IsEnabled(Func<T, bool> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => IsEnabled(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> IsHeader(Func<T, bool> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => IsHeader(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> IsTriggered(Func<T, bool> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => IsTriggered(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> ContextMenuItems(Func<T, IEnumerable> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => ContextMenuItems(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> QuickCommand(Func<T, ICommand> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => QuickCommand(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> QuickCommandIconDescription(Func<T, IconDescription> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => QuickCommandIconDescription(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> QuickCommandLabel(Func<T, string> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => QuickCommandLabel(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> QuickCommandToolTip(Func<T, string> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => QuickCommandToolTip(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> DraggedDataProvider(Func<T, Func<DraggedData>> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => DraggedDataProvider(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> DragEnterAction(Func<T, Action<DragEventArgs>> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => DragEnterAction(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> DragOverAction(Func<T, Action<DragEventArgs>> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => DragOverAction(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> DragLeaveAction(Func<T, Action<DragEventArgs>> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => DragLeaveAction(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> DropAction(Func<T, Action<DragEventArgs>> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => DropAction(getter, x => OnPropertyChanged(x, propertyName, notifier));

        static private IObservable<object> OnPropertyChanged(T data, string propertyName, Func<T, INotifyPropertyChanged> notifier)
        {
            INotifyPropertyChanged notifyPropertyChanged = notifier?.Invoke(data) ?? data as INotifyPropertyChanged;
            if (notifyPropertyChanged is null)
                return Observable.Empty<object>();

            return ObservableHelpers.OnPropertyChanged(notifyPropertyChanged, propertyName);
        }

        public TreeViewItemModelBuilder<T> IsEnabled(Func<T, ICommand> commandGetter, Func<T, object> commandParameterGetter = null, bool valueByDefault = true)
            => IsEnabled(x => CanExecuteGetter(x, commandGetter, commandParameterGetter, valueByDefault), x => OnCanExecuteChanged(x, commandGetter));
        public TreeViewItemModelBuilder<T> IsTriggered(Func<T, ICommand> commandGetter, Func<T, object> commandParameterGetter = null, bool valueByDefault = false)
            => IsTriggered(x => CanExecuteGetter(x, commandGetter, commandParameterGetter, valueByDefault), x => OnCanExecuteChanged(x, commandGetter));

        static private bool CanExecuteGetter(T data, Func<T, ICommand> commandGetter, Func<T, object> commandParameterGetter, bool valueByDefault)
        {
            bool result = commandGetter(data)?.CanExecute(commandParameterGetter?.Invoke(data) ?? data) ?? valueByDefault;
            return result;
        }

        static private IObservable<object> OnCanExecuteChanged(T data, Func<T, ICommand> commandGetter)
        {
            ICommand command = commandGetter(data);
            if (command == null)
                return Observable.Empty<object>();

            return ObservableHelpers.OnCanExecuteChanged(command);
        }

        public TreeViewItemModelBuilder<T> DisplayName(Func<T, string> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.DisplayName = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> FontWeight(Func<T, FontWeight> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.FontWeight = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> IconDescription(Func<T, IconDescription> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.IconDescription = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> ChildrenSource(Func<T, IReadOnlyObservableList<object>> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.ChildrenSource = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> IsEnabled(Func<T, bool> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.IsEnabled = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> IsHeader(Func<T, bool> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.IsHeader = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> IsTriggered(Func<T, bool> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.IsTriggered = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> ContextMenuItems(Func<T, IEnumerable> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.ContextMenuItems = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> QuickCommand(Func<T, ICommand> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.QuickCommand = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> QuickCommandIconDescription(Func<T, IconDescription> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.QuickCommandIconDescription = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> QuickCommandLabel(Func<T, string> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.QuickCommandLabel = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> QuickCommandToolTip(Func<T, string> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.QuickCommandToolTip = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> DraggedDataProvider(Func<T, Func<DraggedData>> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.DraggedDataProvider = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> DragEnterAction(Func<T, Action<DragEventArgs>> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.DragEnterAction = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> DragOverAction(Func<T, Action<DragEventArgs>> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.DragOverAction = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> DragLeaveAction(Func<T, Action<DragEventArgs>> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.DragLeaveAction = x);
            return this;
        }

        public TreeViewItemModelBuilder<T> DropAction(Func<T, Action<DragEventArgs>> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.DropAction = x);
            return this;
        }

        private void AddBinding<TValue>(Func<T, TValue> getter, Func<T, IObservable<object>> observableFunc, Action<TreeViewItemModel, TValue> setter)
        {
            _bindingModulesFunc.Add(data => new ObservableBindingModule<TreeViewItemModel, TValue>(BuildObservable(data, getter, observableFunc), setter));
        }

        static private IObservable<TProperty> BuildObservable<TProperty>(T data, Func<T, TProperty> getter, Func<T, IObservable<object>> observableFunc)
        {
            return observableFunc?.Invoke(data).Select(x => getter(data)).StartWith(getter(data)) ?? Observable.Return(getter(data));
        }

        private Func<ITreeViewItemModel, bool> _canInlineChild;

        public TreeViewItemModelBuilder<T> CanInlineChild(Func<ITreeViewItemModel, bool> canInlineChild)
        {
            _canInlineChild = canInlineChild;
            return this;
        }

        public ITreeViewItemModel Build(T data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            var treeViewItemModel = new TreeViewItemModel(data, _bindingModulesFunc.Select(x => x(data)), synchronizerConfiguration);

            if (_canInlineChild != null)
                return new InlinableTreeViewItemModel<T>(treeViewItemModel, _canInlineChild);
            
            return treeViewItemModel;
        }

        private class TreeViewItemModel : TreeViewItemModel<T>
        {
            private readonly BindingManager<TreeViewItemModel> _bindingManager;

            public TreeViewItemModel(
                T data,
                IEnumerable<IBindingModule<TreeViewItemModel>> bindingModules,
                ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration
            ) : base(data, synchronizerConfiguration)
            {
                _bindingManager = new BindingManager<TreeViewItemModel>();
                _bindingManager.Modules.AddRange(bindingModules);

                _bindingManager.InitializeView(this);
                _bindingManager.BindView(this);
            }

            public override void Dispose()
            {
                _bindingManager.UnbindView();
                base.Dispose();
            }
        }

        private class ObservableBindingModule<TView, TProperty> : IBindingModule<TView>
        {
            private readonly IObservable<TProperty> _observable;
            private readonly Action<TView, TProperty> _setter;
            private IDisposable _subscription;

            public ObservableBindingModule(IObservable<TProperty> observable, Action<TView, TProperty> setter)
            {
                _observable = observable;
                _setter = setter;
            }

            public void InitializeView(TView view)
            {
            }

            public void BindView(TView view)
            {
                if (_observable != null)
                    _subscription = _observable.Subscribe(x => _setter(view, x));
            }

            public void UnbindView()
            {
                _subscription?.Dispose();
                _subscription = null;
            }
        }
    }
}