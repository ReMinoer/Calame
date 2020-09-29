using System.Linq;

namespace Calame.Icons
{
    public class IconDescriptor : IIconDescriptor
    {
        private readonly IIconDescriptorModule[] _modules;
        private readonly IDefaultIconDescriptorModule[] _defaultModules;
        private readonly IBaseTypeIconDescriptorModule[] _baseTypeModules;

        public IconDescriptor(IIconDescriptorModule[] modules, IDefaultIconDescriptorModule[] defaultModules, IBaseTypeIconDescriptorModule[] baseTypeModules)
        {
            _modules = modules;
            _defaultModules = defaultModules;
            _baseTypeModules = baseTypeModules;
        }

        public IconDescription GetIcon(object model)
        {
            IconDescription icon = _modules.Select(x => x.GetIcon(model)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            icon = _defaultModules.Select(x => x.GetDefaultIcon(model)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            return _baseTypeModules.Select(x => x.GetBaseTypeIcon(model)).FirstOrDefault(x => x.Defined);
        }
    }

    public class IconDescriptor<T> : IIconDescriptor<T>
    {
        private readonly IIconDescriptorModule<T>[] _modules;
        private readonly IDefaultIconDescriptorModule<T>[] _defaultModules;

        public IconDescriptor(IIconDescriptorModule<T>[] modules, IDefaultIconDescriptorModule<T>[] defaultModules)
        {
            _modules = modules;
            _defaultModules = defaultModules;
        }

        public IconDescription GetIcon(T model)
        {
            IconDescription icon = _modules.Select(x => x.GetIcon(model)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            return _defaultModules.Select(x => x.GetDefaultIcon(model)).FirstOrDefault(x => x.Defined);
        }

        IconDescription IIconDescriptor.GetIcon(object model) => model is T obj ? GetIcon(obj) : IconDescription.None;
    }
}