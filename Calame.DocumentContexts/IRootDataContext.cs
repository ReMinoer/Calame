using System.Collections.Generic;
using Glyph.Composition.Modelization;

namespace Calame.DocumentContexts
{
    public interface IRootDataContext
    {
        IEnumerable<IGlyphData> RootData { get; }
    }
}