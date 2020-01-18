using Glyph.Composition;

namespace Calame
{
    public interface IComponentFilter
    {
        bool Filter(IGlyphComponent component);
    }
}