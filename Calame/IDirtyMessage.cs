using System.Collections;

namespace Calame
{
    public interface IDirtyMessage
    {
        IDocumentContext DocumentContext { get; }
        IEnumerable DirtyObjects { get; }
    }
}