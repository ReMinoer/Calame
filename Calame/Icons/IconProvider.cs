using System;
using System.ComponentModel.Composition;
using System.Drawing;
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

        public Control GetControl(IconDescription iconDescription, int size) => GetModule(iconDescription)?.GetControl(iconDescription, size);
        public Bitmap GetBitmap(IconDescription iconDescription, int size) => GetModule(iconDescription)?.GetBitmap(iconDescription, size);
        public Uri GetUri(IconDescription iconDescription, int size) => GetModule(iconDescription)?.GetUri(iconDescription, size);

        private IIconProviderModule GetModule(IconDescription iconDescription) => _modules.FirstOrDefault(x => x.Handle(iconDescription));
    }
}