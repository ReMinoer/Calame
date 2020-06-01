using System.ComponentModel.Composition;
using System.Linq;

namespace Calame.Icons
{
    [Export(typeof(IIconDescriptorManager))]
    public class IconDescriptorManager : IIconDescriptorManager
    {
        private readonly IIconDescriptorModule[] _modules;
        private readonly IDefaultIconDescriptorModule[] _defaultModules;

        [ImportingConstructor]
        public IconDescriptorManager([ImportMany] IIconDescriptorModule[] modules, [ImportMany] IDefaultIconDescriptorModule[] defaultModules)
        {
            _modules = modules;
            _defaultModules = defaultModules;
        }

        public IIconDescriptor GetDescriptor()
        {
            return new IconDescriptor(_modules, _defaultModules);
        }

        public IIconDescriptor<T> GetDescriptor<T>()
        {
            return new IconDescriptor<T>(_modules.OfType<IIconDescriptorModule<T>>().ToArray(), _defaultModules.OfType<IDefaultIconDescriptorModule<T>>().ToArray());
        }
    }
}