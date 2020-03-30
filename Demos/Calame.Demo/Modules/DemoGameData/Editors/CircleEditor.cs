using System.ComponentModel.Composition;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.IO;
using Niddle;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditorSource))]
    public class CircleEditorSource : SerializingEditorSource<CircleData, CircleEditor>
    {
        protected override ISerializationFormat<CircleData> SerializationFormat => new DataContractSerializationFormat<CircleData>("Circle", ".circle");
        
        [ImportingConstructor]
        public CircleEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider)
        {
        }
    }
    
    public class CircleEditor : SerializingViewerEditorBase<CircleData>
    {
        public override string ContentPath => null;

        public override void RegisterDependencies(IDependencyRegistry registry)
        {
            base.RegisterDependencies(registry);
            registry.Add(Dependency.OnType<CircleObject>());
        }
    }
}