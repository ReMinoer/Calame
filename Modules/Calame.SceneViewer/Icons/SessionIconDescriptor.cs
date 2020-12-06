using System.ComponentModel.Composition;
using Calame.Icons;
using Calame.Icons.Base;

namespace Calame.SceneViewer.Icons
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<ISession>))]
    public class SessionIconDescriptor : ReTargetingDefaultDescriptorModuleBase<ISession, CalameIconKey>
    {
        protected override CalameIconKey GetTarget(ISession model) => CalameIconKey.SessionMode;
    }
}