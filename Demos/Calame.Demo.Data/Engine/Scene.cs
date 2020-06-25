using Glyph.Composition;
using Glyph.Core;

namespace Calame.Demo.Data.Engine
{
    public class Scene : GlyphObject
    {
        private IGlyphComponent _mainShape;
        public IGlyphComponent MainShape
        {
            get => _mainShape;
            set
            {
                if (_mainShape == value)
                    return;

                if (_mainShape != null)
                    Remove(_mainShape);

                _mainShape = value;

                if (_mainShape != null)
                    Add(_mainShape);
            }
        }

        public Scene(GlyphResolveContext context)
            : base(context)
        {
            Add<SceneNode>();
        }
    }
}