using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Calame.Icons;
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

        private readonly IIconProvider _iconProvider;
        private readonly IIconDescriptor _iconDescriptor;

        private Uri _iconSource;
        public override sealed Uri IconSource => _iconSource;
        protected virtual object IconKey { get; }

        public THandledDocument CurrentDocument { get; private set; }

        public override double PreferredWidth => PreferredLocation == PaneLocation.Right ? 450 : 350;
        public override double PreferredHeight => 300;

        protected CalameTool(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
        {
            Shell = shell;
            EventAggregator = eventAggregator;

            _iconProvider = iconProvider;
            _iconDescriptor = iconDescriptorManager.GetDescriptor();

            if (shell.ActiveItem is THandledDocument document)
            {
                CurrentDocument = document;
                OnDocumentActivated(CurrentDocument);
            }

            Shell.ActiveDocumentChanged += ShellOnActiveDocumentChanged;
            EventAggregator.SubscribeOnUI(this);

            RefreshIcon();
        }

        private void RefreshIcon()
        {
            Uri uri = _iconProvider.GetUri(_iconDescriptor.GetIcon(IconKey ?? this), 16);
            Set(ref _iconSource, uri, nameof(IconSource));
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

            OnDocumentsCleaned();
            CurrentDocument = null;
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