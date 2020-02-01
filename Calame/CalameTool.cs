using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame
{
    public abstract class CalameTool<THandledDocument> : Tool, IHandle<IDocumentContext>, IDisposable
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
            EventAggregator.SubscribeOnUIThread(this);
        }

        public void Dispose()
        {
            Shell.ActiveDocumentChanged -= ShellOnActiveDocumentChanged;
            EventAggregator.Unsubscribe(this);
        }

        private void ShellOnActiveDocumentChanged(object sender, EventArgs e)
        {
            if (Shell.ActiveItem != null)
                return;

            CurrentDocument = null;
            OnDocumentsCleaned();
        }

        public async Task HandleAsync(IDocumentContext message, CancellationToken cancellationToken)
        {
            if (message is THandledDocument handledDocument)
            {
                CurrentDocument = handledDocument;
                await OnDocumentActivated(CurrentDocument);
            }
            else
            {
                CurrentDocument = null;
                await OnDocumentsCleaned();
            }
        }
        
        protected abstract Task OnDocumentActivated(THandledDocument activeDocument);
        protected abstract Task OnDocumentsCleaned();

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