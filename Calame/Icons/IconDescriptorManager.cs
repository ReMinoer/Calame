using System.ComponentModel.Composition;
using System.Linq;

namespace Calame.Icons
{
    [Export(typeof(IIconDescriptorManager))]
    public class IconDescriptorManager : IIconDescriptorManager
    {
        private readonly IIconDescriptorModule[] _modules;
        
        [ImportingConstructor]
        public IconDescriptorManager([ImportMany] IIconDescriptorModule[] modules)
        {
            _modules = modules;
        }

        public IIconDescriptor<T> GetDescriptor<T>()
        {
            return new IconDescriptor<T>(_modules.OfType<IIconDescriptorModule<T>>().ToArray(), _modules.OfType<IDefaultIconDescriptorModule<T>>().Single());
        }

        private class IconDescriptor<T> : IIconDescriptor<T>
        {
            private readonly IIconDescriptorModule<T>[] _modules;
            private readonly IDefaultIconDescriptorModule<T> _defaultModule;

            public IconDescriptor(IIconDescriptorModule<T>[] modules, IDefaultIconDescriptorModule<T> defaultModule)
            {
                _modules = modules;
                _defaultModule = defaultModule;
            }

            public IconDescription GetIcon(T model)
            {
                IconDescription icon = _modules.Select(x => x.GetIcon(model)).FirstOrDefault(x => x.Defined);
                return icon.Defined ? icon : _defaultModule.GetDefaultIcon(model);
            }

            IconDescription IIconDescriptor.GetIcon(object model) => model is T obj ? GetIcon(obj) : IconDescription.None;
        }
    }
}