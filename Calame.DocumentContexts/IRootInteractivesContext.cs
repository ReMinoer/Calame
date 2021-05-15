using System.Collections.Generic;
using Fingear.Interactives;

namespace Calame.DocumentContexts
{
    public interface IRootInteractivesContext
    {
        IEnumerable<IInteractive> RootInteractives { get; }
    }
}