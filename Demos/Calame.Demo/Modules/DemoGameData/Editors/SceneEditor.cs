using System.ComponentModel.Composition;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.IO;
using Niddle;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditorSource))]
    public class SceneEditorSource : SerializingEditorSource<SceneData, SceneEditor>
    {
        protected override ISerializationFormat<SceneData> SerializationFormat => new DataContractSerializationFormat<SceneData>("Scene", ".scene");
        
        [ImportingConstructor]
        public SceneEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider)
        {
        }
    }
    
    public class SceneEditor : SerializingViewerEditorBase<SceneData>
    {
        public override string ContentPath => null;

        public override void RegisterDependencies(IDependencyRegistry registry)
        {
            base.RegisterDependencies(registry);
            registry.Add(Dependency.OnType<Scene>());
            registry.Add(Dependency.OnType<RectangleObject>());
            registry.Add(Dependency.OnType<CircleObject>());
        }
    }
}