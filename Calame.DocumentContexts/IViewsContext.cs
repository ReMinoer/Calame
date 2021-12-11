using System.Collections.Generic;
using Glyph.Core;

namespace Calame.DocumentContexts
{
    public interface IViewsContext
    {
        IEnumerable<IView> Views { get; }
    }
}