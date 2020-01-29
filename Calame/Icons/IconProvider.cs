using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Calame.Icons
{
    [Export(typeof(IIconProvider))]
    public class IconProvider : IIconProvider
    {
        private readonly IIconProviderModule[] _modules;
        
        [ImportingConstructor]
        public IconProvider([ImportMany] IIconProviderModule[] modules)
        {
            _modules = modules;
        }

        public Control GetControl(IconDescription iconDescription, int size)
        {
            return _modules.FirstOrDefault(x => x.Handle(iconDescription))?.GetControl(iconDescription, size);
        }
    }
}