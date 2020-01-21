using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame
{
    public abstract class CalameTool<THandledDocument> : Tool, IHandle<THandledDocument>, IDisposable
        where THandledDocument : class, IDocumentContext
    {
        protected readonly IEventAggregator EventAggregator;
        protected readonly IShell Shell;

        public THandledDocument CurrentDocument { get; private set; }

        protected CalameTool(IShell shell, IEventAggregator eventAggregator)
        {
            Shell = shell;
            EventAggregator = eventAggregator;
            
            Shell.ActiveDocumentChanged += ShellOnActiveDocumentChanged;
            EventAggregator.Subscribe(this);
        }

        public void Dispose()
        {
            Shell.ActiveDocumentChanged -= ShellOnActiveDocumentChanged;
            EventAggregator.Unsubscribe(this);
        }

        private void ShellOnActiveDocumentChanged(object sender, EventArgs e)
        {
            if (Shell.ActiveItem == null)
                OnDocumentsCleaned();
        }

        void IHandle<THandledDocument>.Handle(THandledDocument message)
        {
            CurrentDocument = message;
            OnDocumentActivated(CurrentDocument);
        }
        
        protected abstract void OnDocumentActivated(THandledDocument activeDocument);
        protected abstract void OnDocumentsCleaned();

        protected bool SetValue<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            field = newValue;
            NotifyOfPropertyChange(propertyName);
            return true;
        }
    }
}