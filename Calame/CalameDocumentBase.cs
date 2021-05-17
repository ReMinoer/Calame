using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework;
using Microsoft.Extensions.Logging;

namespace Calame
{
    public abstract class CalameDocumentBase : Document
    {
        protected readonly IEventAggregator EventAggregator;
        protected readonly ILogger Logger;
        private readonly IIconProvider _iconProvider;
        private readonly IIconDescriptor _iconDescriptor;

        private Uri _iconSource;
        public override sealed Uri IconSource => _iconSource;
        protected virtual object IconKey { get; }

        public CalameDocumentBase(IEventAggregator eventAggregator, ILoggerProvider loggerProvider, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
        {
            EventAggregator = eventAggregator;
            EventAggregator.SubscribeOnUI(this);

            Logger = loggerProvider.CreateLogger(Id.ToString());

            _iconProvider = iconProvider;
            _iconDescriptor = iconDescriptorManager.GetDescriptor();

            RefreshIcon();
        }

        protected void RefreshIcon()
        {
            Uri uri = _iconProvider.GetUri(_iconDescriptor.GetIcon(IconKey ?? this), 16);
            Set(ref _iconSource, uri, nameof(IconSource));
        }

        protected abstract Task DisposeDocumentAsync();

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (!await base.CanCloseAsync(cancellationToken))
                return false;

            await DisposeDocumentAsync();
            return true;
        }
    }
}