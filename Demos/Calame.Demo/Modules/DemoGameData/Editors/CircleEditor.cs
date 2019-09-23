using System.ComponentModel.Composition;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.IO;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditor))]
    public class CircleEditor : SerializingViewerEditorBase<CircleData>
    {
        public override string ContentPath => null;
        protected override ISerializationFormat<CircleData> SerializationFormat => new DataContractSerializationFormat<CircleData>("Circle", ".circle");

        [ImportingConstructor]
        public CircleEditor(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider)
        {
        }
    }
}