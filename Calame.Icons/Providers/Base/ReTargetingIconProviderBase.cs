using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;

namespace Calame.Icons.Providers.Base
{
    public abstract class ReTargetingIconProviderBase<TKey, TTarget> : IIconProviderModule<TKey>
    {
        private readonly IIconProviderModule<TTarget>[] _iconProviders;

        protected ReTargetingIconProviderBase()
        {
            _iconProviders = IoC.GetAll<IIconProviderModule<TTarget>>()?.ToArray();
        }

        public bool Handle(IconDescription iconDescription) => iconDescription.Key is TKey;

        public Control GetControl(IconDescription iconDescription, int size) => GetValidProviders(iconDescription).Select(x => x.GetControl(GetTargetDescription(iconDescription), size)).FirstOrDefault();
        public Bitmap GetBitmap(IconDescription iconDescription, int size) => GetValidProviders(iconDescription).Select(x => x.GetBitmap(GetTargetDescription(iconDescription), size)).FirstOrDefault();
        public Uri GetUri(IconDescription iconDescription, int size) => GetValidProviders(iconDescription).Select(x => x.GetUri(GetTargetDescription(iconDescription), size)).FirstOrDefault();

        private IEnumerable<IIconProviderModule<TTarget>> GetValidProviders(IconDescription iconDescription) => _iconProviders.Where(x => x.Handle(GetTargetDescription(iconDescription)));
        private IconDescription GetTargetDescription(IconDescription iconDescription) => new IconDescription(GetTargetKey((TKey)iconDescription.Key), iconDescription.Brush);
        protected abstract TTarget GetTargetKey(TKey key);
    }
}