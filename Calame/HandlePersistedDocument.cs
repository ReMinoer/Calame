using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using Gemini.Framework;

namespace Calame
{
    public abstract class HandlePersistedDocument : PersistedDocument
    {
        protected readonly IEventAggregator EventAggregator;

        protected HandlePersistedDocument(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
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