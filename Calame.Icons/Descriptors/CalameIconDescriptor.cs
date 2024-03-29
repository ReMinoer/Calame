﻿using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<CalameIconKey>))]
    public class CalameIconDescriptor : DefaultIconDescriptorModuleBase<CalameIconKey>
    {
        static private readonly Brush GreenBrush = new SolidColorBrush(Color.FromRgb(50, 124, 46));
        static private readonly Brush BlueBrush = new SolidColorBrush(Color.FromRgb(0, 74, 140));
        static private readonly Brush RedBrush = new SolidColorBrush(Color.FromRgb(145, 33, 11));

        static CalameIconDescriptor()
        {
            GreenBrush.Freeze();
            BlueBrush.Freeze();
            RedBrush.Freeze();
        }

        public override IconDescription GetDefaultIcon(CalameIconKey key)
        {
            return new IconDescription(key, GetBrush(key), GetPadding(key));
        }

        private Brush GetBrush(CalameIconKey key)
        {
            switch (key)
            {
                case CalameIconKey.Play:
                    return GreenBrush;
                case CalameIconKey.Pause:
                case CalameIconKey.NextFrame:
                    return BlueBrush;
                case CalameIconKey.Stop:
                    return RedBrush;
                default:return IconBrushes.Default;
            }
        }

        private double GetPadding(CalameIconKey key)
        {
            switch (key)
            {
                case CalameIconKey.Play:
                case CalameIconKey.NextFrame:
                case CalameIconKey.Pause:
                case CalameIconKey.Stop:
                case CalameIconKey.Select:
                    return 2;
                case CalameIconKey.Reopen:
                    return 1.5;
                case CalameIconKey.Reset:
                case CalameIconKey.CheckUpdates:
                    return 1;
                default: return 0;
            }
        }
    }
}