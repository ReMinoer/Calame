using System.ComponentModel.Composition;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.IO;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditor))]
    public class RectangleEditor : SerializingViewerEditorBase<RectangleData>
    {
        public override string ContentPath => "Content/";
        protected override ISerializationFormat<RectangleData> SerializationFormat => new DataContractSerializationFormat<RectangleData>("Rectangle", ".rectangle");

        [ImportingConstructor]
        public RectangleEditor(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider)
        {
        }
    }
}