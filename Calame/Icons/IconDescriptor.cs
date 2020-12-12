using System;
using System.Linq;

namespace Calame.Icons
{
    public class IconDescriptor : IIconDescriptor
    {
        private readonly IIconDescriptorModule[] _modules;
        private readonly IDefaultIconDescriptorModule[] _defaultModules;
        private readonly ITypeIconDescriptorModule[] _typeModules;
        private readonly ITypeDefaultIconDescriptorModule[] _typeDefaultModules;
        private readonly IFallbackIconDescriptorModule[] _fallbackModules;

        public IconDescriptor(IIconDescriptorModule[] modules, IDefaultIconDescriptorModule[] defaultModules,
            ITypeIconDescriptorModule[] typeModules, ITypeDefaultIconDescriptorModule[] typeDefaultModules,
            IFallbackIconDescriptorModule[] fallbackModules)
        {
            _modules = modules;
            _defaultModules = defaultModules;
            _typeModules = typeModules;
            _typeDefaultModules = typeDefaultModules;
            _fallbackModules = fallbackModules;
        }

        public IconDescription GetIcon(object model)
        {
            IconDescription icon = _modules.Where(x => x.Handle(model)).Select(x => x.GetIcon(model)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            icon = _defaultModules.Where(x => x.Handle(model)).Select(x => x.GetDefaultIcon(model)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            return _fallbackModules.Where(x => x.Handle(model)).Select(x => x.GetBaseTypeIcon(model)).FirstOrDefault(x => x.Defined);
        }

        public IconDescription GetTypeIcon(Type type)
        {
            IconDescription icon = _typeModules.Where(x => x.Handle(type)).Select(x => x.GetTypeIcon(type)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            icon = _typeDefaultModules.Where(x => x.Handle(type)).Select(x => x.GetTypeDefaultIcon(type)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            return _fallbackModules.Where(x => x.Handle(type)).Select(x => x.GetBaseTypeIcon(type)).FirstOrDefault(x => x.Defined);
        }
    }

    public class IconDescriptor<T> : IIconDescriptor<T>
    {
        private readonly IIconDescriptorModule<T>[] _modules;
        private readonly IDefaultIconDescriptorModule<T>[] _defaultModules;
        private readonly ITypeIconDescriptorModule<T>[] _typeModules;
        private readonly ITypeDefaultIconDescriptorModule<T>[] _typeDefaultModules;

        public IconDescriptor(IIconDescriptorModule<T>[] modules, IDefaultIconDescriptorModule<T>[] defaultModules,
            ITypeIconDescriptorModule<T>[] typeModules, ITypeDefaultIconDescriptorModule<T>[] typeDefaultModules)
        {
            _modules = modules;
            _defaultModules = defaultModules;
            _typeModules = typeModules;
            _typeDefaultModules = typeDefaultModules;
        }

        public IconDescription GetIcon(T model)
        {
            IconDescription icon = _modules.Where(x => x.Handle(model)).Select(x => x.GetIcon(model)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            return _defaultModules.Where(x => x.Handle(model)).Select(x => x.GetDefaultIcon(model)).FirstOrDefault(x => x.Defined);
        }

        public IconDescription GetTypeIcon(Type type)
        {
            IconDescription icon = _typeModules.Where(x => x.Handle(type)).Select(x => x.GetTypeIcon(type)).FirstOrDefault(x => x.Defined);
            if (icon.Defined)
                return icon;

            return _typeDefaultModules.Where(x => x.Handle(type)).Select(x => x.GetTypeDefaultIcon(type)).FirstOrDefault(x => x.Defined);
        }

        IconDescription IIconDescriptor.GetIcon(object model) => model is T obj ? GetIcon(obj) : IconDescription.None;
    }
}