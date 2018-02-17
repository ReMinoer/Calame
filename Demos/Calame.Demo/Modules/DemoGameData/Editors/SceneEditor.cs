using System.ComponentModel.Composition;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.IO;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditor))]
    public class SceneEditor : SerializingViewerEditorBase<SceneData>
    {
        public override string ContentPath => "Content/";
        protected override ISerializationFormat<SceneData> SerializationFormat => new DataContractSerializationFormat<SceneData>("Scene", ".scene");

        [ImportingConstructor]
        public SceneEditor(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider)
        {
        }
    }
}