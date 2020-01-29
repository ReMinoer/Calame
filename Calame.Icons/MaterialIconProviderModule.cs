﻿using System.ComponentModel.Composition;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace Calame.Icons
{
    [Export(typeof(IIconProviderModule))]
    public class MaterialIconProviderModule : IIconProviderModule
    {
        public bool Handle(IconDescription iconDescription)
        {
            return iconDescription.Key is PackIconMaterialKind;
        }

        public Control GetControl(IconDescription iconDescription, int size)
        {
            return new PackIconMaterial
            {
                Kind = (PackIconMaterialKind)iconDescription.Key,
                Foreground = iconDescription.Brush,
                Width = size,
                Height = size
            };
        }
    }
}