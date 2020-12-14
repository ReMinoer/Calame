using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Calame.Icons.Base
{
    [Export(typeof(ReTargetIconDescriptorManager))]
    public class ReTargetIconDescriptorManager : IconDescriptorManager
    {
        static private ReTargetIconDescriptorManager _instance;
        static public ReTargetIconDescriptorManager Instance => _instance ?? (_instance = IoC.Get<ReTargetIconDescriptorManager>());

        [ImportingConstructor]
        public ReTargetIconDescriptorManager([ImportMany] IIconDescriptorModule[] modules, [ImportMany] IDefaultIconDescriptorModule[] defaultModules,
            [ImportMany] ITypeIconDescriptorModule[] typeModules, [ImportMany] ITypeDefaultIconDescriptorModule[] typeDefaultModules)
            : base(modules, defaultModules, typeModules, typeDefaultModules, Array.Empty<IFallbackIconDescriptorModule>())
        {
        }
    }
}