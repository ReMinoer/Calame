﻿using System;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework;

namespace Calame
{
    public abstract class CalameDocumentBase : Document
    {
        protected readonly IEventAggregator EventAggregator;
        private readonly IIconProvider _iconProvider;
        private readonly IIconDescriptor _iconDescriptor;

        private BitmapImage _bitmapImage;
        public override sealed ImageSource IconSource => _bitmapImage;
        protected virtual object IconKey { get; }

        public CalameDocumentBase(IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
        {
            EventAggregator = eventAggregator;
            EventAggregator.SubscribeOnUI(this);

            _iconProvider = iconProvider;
            _iconDescriptor = iconDescriptorManager.GetDescriptor();

            RefreshIcon();
        }

        protected void RefreshIcon()
        {
            BitmapImage bitmapImage = null;

            Uri uri = _iconProvider.GetUri(_iconDescriptor.GetIcon(IconKey ?? this), 16);
            if (uri != null)
            {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = uri;
                bitmapImage.EndInit();
            }

            Set(ref _bitmapImage, bitmapImage, nameof(IconSource));
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