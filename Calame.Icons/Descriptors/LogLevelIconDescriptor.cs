using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using MahApps.Metro.IconPacks;
using Microsoft.Extensions.Logging;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<LogLevel>))]
    public class LogLevelIconDescriptor : DefaultIconDescriptorModuleBase<LogLevel>
    {
        public override IconDescription GetDefaultIcon(LogLevel key)
        {
            switch (key)
            {
                case LogLevel.Trace:
                    return new IconDescription(PackIconMaterialKind.MapMarker, Brushes.DimGray);
                case LogLevel.Debug:
                    return new IconDescription(PackIconMaterialKind.Ladybug, Brushes.DarkGreen);
                case LogLevel.Information:
                    return new IconDescription(PackIconMaterialKind.Information, Brushes.SteelBlue);
                case LogLevel.Warning:
                    return new IconDescription(PackIconMaterialKind.Alert, Brushes.DarkGoldenrod);
                case LogLevel.Error:
                    return new IconDescription(PackIconMaterialKind.AlertCircle, Brushes.DarkRed);
                case LogLevel.Critical:
                    return new IconDescription(PackIconMaterialKind.AlertBox, Brushes.DarkRed);
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}