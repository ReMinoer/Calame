﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Calame.Icons;
using Calame.UserControls;
using Diese.Collections.Observables.ReadOnly;
using Simulacra.Binding;

namespace Calame.Utils
{
    public class TreeViewItemModelBuilder<T>
    {
        private readonly List<Func<T, IBindingModule<TreeViewItemModel>>> _bindingModulesFunc = new List<Func<T, IBindingModule<TreeViewItemModel>>>();
        
        public TreeViewItemModelBuilder<T> DisplayName(Func<T, string> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => DisplayName(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> IconDescription(Func<T, IconDescription> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => IconDescription(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> ChildrenSource(Func<T, IReadOnlyObservableList<object>> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => ChildrenSource(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> IsEnabled(Func<T, bool> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => IsEnabled(getter, x => OnPropertyChanged(x, propertyName, notifier));
        public TreeViewItemModelBuilder<T> IsTriggered(Func<T, bool> getter, string propertyName, Func<T, INotifyPropertyChanged> notifier = null)
            => IsTriggered(getter, x => OnPropertyChanged(x, propertyName, notifier));

        static private IObservable<object> OnPropertyChanged(T data, string propertyName, Func<T, INotifyPropertyChanged> notifier)
        {
            return ObservableHelpers.OnPropertyChanged(notifier?.Invoke(data) ?? data as INotifyPropertyChanged, propertyName);
        }
        
        public TreeViewItemModelBuilder<T> DisplayName(Func<T, string> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.DisplayName = x);
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

        public TreeViewItemModelBuilder<T> IsTriggered(Func<T, bool> getter, Func<T, IObservable<object>> observableFunc = null)
        {
            AddBinding(getter, observableFunc, (v, x) => v.IsTriggered = x);
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

        public ITreeViewItemModel Build(ITreeContext treeContext, T data)
        {
            return new TreeViewItemModel(treeContext, data, _bindingModulesFunc.Select(x => x(data)));
        }

        private class TreeViewItemModel : TreeViewItemModel<T>
        {
            private readonly BindingManager<TreeViewItemModel> _bindingManager;

            public TreeViewItemModel(ITreeContext treeContext, T data, IEnumerable<IBindingModule<TreeViewItemModel>> bindingModules)
                : base(treeContext, data)
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