using System.Collections.Generic;
using Glyph;

namespace Calame.DocumentContexts
{
    public interface IRootScenesContext
    {
        IEnumerable<ISceneNode> RootScenes { get; }
    }
}