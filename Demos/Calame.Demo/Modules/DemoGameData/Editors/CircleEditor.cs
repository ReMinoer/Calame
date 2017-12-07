using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.DataModelViewer;
using Calame.Demo.Modules.DemoGameData.GameData;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.IO;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditor))]
    public class CircleEditor : ViewerEditorBase<CircleData>
    {
        public override string ContentPath => "Content/";
        protected override ISaveLoadFormat<CircleData> SaveLoadFormat => new XmlSerializationFormat<CircleData>("Circle", ".circle");
        public override async Task<IGlyphCreator<IGlyphComponent>> NewDataAsync() => new CircleData();
    }
}