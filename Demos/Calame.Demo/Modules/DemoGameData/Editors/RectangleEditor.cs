using System.ComponentModel.Composition;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.IO;
using Niddle;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditorSource))]
    public class RectangleEditorSource : SerializingEditorSource<RectangleData, RectangleEditor>
    {
        protected override ISerializationFormat<RectangleData> SerializationFormat => new DataContractSerializationFormat<RectangleData>("Rectangle", ".rectangle");
        
        [ImportingConstructor]
        public RectangleEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider)
        {
        }
    }
    
    public class RectangleEditor : SerializingViewerEditorBase<RectangleData>
    {
        public override string ContentPath => null;

        public override void RegisterDependencies(IDependencyRegistry registry)
        {
            base.RegisterDependencies(registry);
            registry.Add(Dependency.OnType<RectangleObject>());
        }
    }
}