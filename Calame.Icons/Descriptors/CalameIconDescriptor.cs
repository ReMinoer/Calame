using System.ComponentModel.Composition;
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
                    return 2;
                case CalameIconKey.ResetSession:
                    return 1;
                default: return 0;
            }
        }
    }
}