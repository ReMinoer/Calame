using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<CalameIconKey>))]
    public class CalameIconDescriptor : DefaultIconDescriptorModuleBase<CalameIconKey>
    {
        public override IconDescription GetDefaultIcon(CalameIconKey key)
        {
            return new IconDescription(key, GetBrush(key));
        }

        private Brush GetBrush(CalameIconKey key)
        {
            switch (key)
            {
                case CalameIconKey.Play: return Brushes.Green;
                case CalameIconKey.Pause: return Brushes.CornflowerBlue;
                case CalameIconKey.Stop: return Brushes.DarkRed;
                default: return IconBrushes.Default;
            }
        }
    }
}