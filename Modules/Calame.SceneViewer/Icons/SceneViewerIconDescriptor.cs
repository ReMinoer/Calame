using System.ComponentModel.Composition;
using Calame.Icons;
using Calame.Icons.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework;

namespace Calame.SceneViewer.Icons
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<IDocument>))]
    public class SceneViewerIconDescriptor : ReTargetingDefaultDescriptorModuleBase<IDocument, IDataSession>
    {
        protected override IDataSession GetTarget(IDocument model) => (model as SceneViewerViewModel)?.Session as IDataSession;
    }
}