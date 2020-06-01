using System.Linq;

namespace Calame.Icons
{
    public class IconDescriptor : IIconDescriptor
    {
        private readonly IIconDescriptorModule[] _modules;
        private readonly IDefaultIconDescriptorModule[] _defaultModules;

        public IconDescriptor(IIconDescriptorModule[] modules, IDefaultIconDescriptorModule[] defaultModules)
        {
            _modules = modules;
            _defaultModules = defaultModules;
        }

        public IconDescription GetIcon(object model)
        {
            IconDescription icon = _modules.Select(x => x.GetIcon(model)).FirstOrDefault(x => x.Defined);
            return icon.Defined ? icon : _defaultModules.Select(x => x.GetDefaultIcon(model)).FirstOrDefault(x => x.Defined);
        }
    }

    public class IconDescriptor<T> : IIconDescriptor<T>
    {
        private readonly IIconDescriptorModule<T>[] _modules;
        private readonly IDefaultIconDescriptorModule<T>[] _defaultModule;

        public IconDescriptor(IIconDescriptorModule<T>[] modules, IDefaultIconDescriptorModule<T>[] defaultModule)
        {
            _modules = modules;
            _defaultModule = defaultModule;
        }

        public IconDescription GetIcon(T model)
        {
            IconDescription icon = _modules.Select(x => x.GetIcon(model)).FirstOrDefault(x => x.Defined);
            return icon.Defined ? icon : _defaultModule.Select(x => x.GetDefaultIcon(model)).FirstOrDefault(x => x.Defined);
        }

        IconDescription IIconDescriptor.GetIcon(object model) => model is T obj ? GetIcon(obj) : IconDescription.None;
    }
}