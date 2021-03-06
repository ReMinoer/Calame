﻿namespace Calame.Icons
{
    public interface IIconProviderModule : IIconProvider
    {
        bool Handle(IconDescription iconDescription);
    }

    public interface IIconProviderModule<TKey> : IIconProviderModule
    {
    }
}