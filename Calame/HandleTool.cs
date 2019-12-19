using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using Gemini.Framework;

namespace Calame
{
    public abstract class HandleTool : Tool, IHandle<IDocumentContext>, IDisposable
    {
        protected readonly IEventAggregator EventAggregator;
        public IDocumentContext CurrentDocument { get; private set; }

        protected HandleTool(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }

        public void Handle(IDocumentContext message)
        {
            CurrentDocument = message;
        }

        protected bool SetValue<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            field = newValue;
            NotifyOfPropertyChange(propertyName);
            return true;
        }

        public void Dispose() => EventAggregator.Unsubscribe(this);
    }
}