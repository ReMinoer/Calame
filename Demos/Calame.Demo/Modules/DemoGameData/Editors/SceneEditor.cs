using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.Composition.Modelization;
using Glyph.IO;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditor))]
    public class SceneEditor : ViewerEditorBase<SceneData>
    {
        public override string ContentPath => "Content/";
        protected override ISaveLoadFormat<SceneData> SaveLoadFormat => new XmlSerializationFormat<SceneData>("Scene", ".scene");
        public override async Task<IGlyphCreator> NewDataAsync() => new SceneData();
    }
}