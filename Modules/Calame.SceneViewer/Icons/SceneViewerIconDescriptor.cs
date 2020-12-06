using System.ComponentModel.Composition;
using Calame.Icons;
using Calame.Icons.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework;

namespace Calame.SceneViewer.Icons
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<IDocument>))]
    public class SceneViewerIconDescriptor : ReTargetingDefaultDescriptorModuleBase<IDocument, ISession>
    {
        protected override ISession GetTarget(IDocument model) => (model as SceneViewerViewModel)?.Session;
    }
}