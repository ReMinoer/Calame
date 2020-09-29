using System.ComponentModel.Composition;
using System.Linq;

namespace Calame.Icons
{
    [Export(typeof(IIconDescriptorManager))]
    public class IconDescriptorManager : IIconDescriptorManager
    {
        private readonly IIconDescriptorModule[] _modules;
        private readonly IDefaultIconDescriptorModule[] _defaultModules;
        private readonly IBaseTypeIconDescriptorModule[] _baseTypeModules;

        [ImportingConstructor]
        public IconDescriptorManager([ImportMany] IIconDescriptorModule[] modules, [ImportMany] IDefaultIconDescriptorModule[] defaultModules, [ImportMany] IBaseTypeIconDescriptorModule[] baseTypeModules)
        {
            _modules = modules;
            _defaultModules = defaultModules;
            _baseTypeModules = baseTypeModules;
        }

        public IIconDescriptor GetDescriptor()
        {
            return new IconDescriptor(_modules, _defaultModules, _baseTypeModules);
        }

        public IIconDescriptor<T> GetDescriptor<T>()
        {
            return new IconDescriptor<T>(_modules.OfType<IIconDescriptorModule<T>>().ToArray(), _defaultModules.OfType<IDefaultIconDescriptorModule<T>>().ToArray());
        }
    }
}