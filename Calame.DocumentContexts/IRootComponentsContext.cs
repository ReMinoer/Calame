using System.Collections.Generic;
using Glyph.Composition;

namespace Calame.DocumentContexts
{
    public interface IRootComponentsContext
    {
        IEnumerable<IGlyphComponent> RootComponents { get; }
    }
}