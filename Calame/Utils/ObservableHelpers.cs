﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Calame.Utils
{
    static public class ObservableHelpers
    {
        static public IObservable<EventPattern<PropertyChangedEventArgs>> OnPropertyChanged(INotifyPropertyChanged notifyPropertyChanged)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => notifyPropertyChanged.PropertyChanged += x,    
                x => notifyPropertyChanged.PropertyChanged -= x);
        }

        static public IObservable<EventPattern<PropertyChangedEventArgs>> OnPropertyChanged(INotifyPropertyChanged notifyPropertyChanged, string propertyName)
        {
            return OnPropertyChanged(notifyPropertyChanged).Where(x => x.EventArgs.PropertyName == propertyName);
        }

        static public IObservable<EventPattern<PropertyChangedEventArgs>> OnPropertyChanged(INotifyPropertyChanged notifyPropertyChanged, params string[] propertyNames)
        {
            return OnPropertyChanged(notifyPropertyChanged).Where(x => propertyNames.Contains(x.EventArgs.PropertyName));
        }

        static public IObservable<EventPattern<EventArgs>> OnCanExecuteChanged(ICommand command)
        {
            return Observable.FromEventPattern<EventHandler, EventArgs>(
                x => command.CanExecuteChanged += x,
                x => command.CanExecuteChanged -= x);
        }
    }
}