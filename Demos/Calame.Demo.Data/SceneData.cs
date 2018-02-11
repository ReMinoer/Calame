using Glyph.Composition.Modelization;
using Glyph.Core;

namespace Calame.Demo.Data
{
    public class SceneData : BindedData<SceneData, GlyphObject>
    {
        public FactoryConfigurator<RectangleData, GlyphObject> Rectangles { get; set; }

        public SceneData()
        {
            SubConfigurators.Add(Rectangles = new FactoryConfigurator<RectangleData, GlyphObject>(this));
        }

        protected override GlyphObject New()
        {
            GlyphObject glyphObject = base.New();
            glyphObject.Add<SceneNode>();
            return glyphObject;
        }
    }
}