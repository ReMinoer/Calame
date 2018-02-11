using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.Composition.Modelization;
using Glyph.IO;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditor))]
    public class RectangleEditor : ViewerEditorBase<RectangleData>
    {
        public override string ContentPath => "Content/";
        protected override ISaveLoadFormat<RectangleData> SaveLoadFormat => new XmlSerializationFormat<RectangleData>("Rectangle", ".rectangle");
        public override async Task<IGlyphCreator> NewDataAsync() => new RectangleData();
    }
}