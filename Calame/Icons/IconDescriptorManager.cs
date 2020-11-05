﻿using System.ComponentModel.Composition;
using System.Linq;

namespace Calame.Icons
{
    [Export(typeof(IIconDescriptorManager))]
    public class IconDescriptorManager : IIconDescriptorManager
    {
        private readonly IIconDescriptorModule[] _modules;
        private readonly IDefaultIconDescriptorModule[] _defaultModules;
        private readonly ITypeIconDescriptorModule[] _typeModules;
        private readonly ITypeDefaultIconDescriptorModule[] _typeDefaultModules;
        private readonly IBaseTypeIconDescriptorModule[] _baseTypeModules;

        [ImportingConstructor]
        public IconDescriptorManager([ImportMany] IIconDescriptorModule[] modules, [ImportMany] IDefaultIconDescriptorModule[] defaultModules,
            [ImportMany] ITypeIconDescriptorModule[] typeModules, [ImportMany] ITypeDefaultIconDescriptorModule[] typeDefaultModules,
            [ImportMany] IBaseTypeIconDescriptorModule[] baseTypeModules)
        {
            _modules = modules;
            _defaultModules = defaultModules;
            _typeModules = typeModules;
            _typeDefaultModules = typeDefaultModules;
            _baseTypeModules = baseTypeModules;
        }

        public IIconDescriptor GetDescriptor()
        {
            return new IconDescriptor(_modules, _defaultModules, _typeModules, _typeDefaultModules, _baseTypeModules);
        }

        public IIconDescriptor<T> GetDescriptor<T>()
        {
            IIconDescriptorModule<T>[] modules =
                _modules.OfType<IIconDescriptorModule<T>>()
                .ToArray();

            IDefaultIconDescriptorModule<T>[] defaultModules =
                _defaultModules.OfType<IDefaultIconDescriptorModule<T>>()
                .ToArray();

            ITypeIconDescriptorModule<T>[] typeModules =
                _typeModules.OfType<ITypeIconDescriptorModule<T>>()
                .Except(modules.OfType<ITypeIconDescriptorModule<T>>())
                .ToArray();

            ITypeDefaultIconDescriptorModule<T>[] typeDefaultModules =
                _typeDefaultModules.OfType<ITypeDefaultIconDescriptorModule<T>>()
                .Except(defaultModules.OfType<ITypeDefaultIconDescriptorModule<T>>())
                .ToArray();

            return new IconDescriptor<T>(modules, defaultModules, typeModules, typeDefaultModules);
        }
    }
}