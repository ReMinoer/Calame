using System.ComponentModel.Composition;
using Calame.DataModelViewer.ViewModels;
using Calame.Icons;
using Calame.Icons.Base;
using Gemini.Framework;

namespace Calame.DataModelViewer.Icons
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<IDocument>))]
    public class DataModelViewerViewerIconDescriptor : ReTargetingDefaultDescriptorModuleBase<IDocument, IEditor>
    {
        protected override IEditor GetTarget(IDocument model) => (model as DataModelViewerViewModel)?.Editor;
    }
}