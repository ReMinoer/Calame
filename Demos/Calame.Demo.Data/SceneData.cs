using Diese.Collections.Observables;
using Glyph.Composition.Modelization;
using Glyph.Core;

namespace Calame.Demo.Data
{
    public class SceneData : BindedData<SceneData, GlyphObject>
    {
        public ObservableCollection<IShapeData> Shapes { get; } = new ObservableCollection<IShapeData>();

        static SceneData()
        {
            CollectionBindings.AddFactory(x => x.Shapes);
        }

        protected override GlyphObject New()
        {
            GlyphObject glyphObject = base.New();
            glyphObject.Add<SceneNode>();
            return glyphObject;
        }
    }
}