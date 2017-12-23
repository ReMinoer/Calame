using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.DataModelViewer;
using Calame.Demo.Modules.DemoGameData.GameData;
using Glyph.Composition;
using Glyph.IO;
using Glyph.Modelization;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditor))]
    public class RectangleEditor : ViewerEditorBase<RectangleData>
    {
        public override string ContentPath => "Content/";
        protected override ISaveLoadFormat<RectangleData> SaveLoadFormat => new XmlSerializationFormat<RectangleData>("Rectangle", ".rectangle");
        public override async Task<IBindedGlyphCreator<IGlyphComponent>> NewDataAsync() => new RectangleData();
    }
}