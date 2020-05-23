using System.Collections;
using Diese.Collections;

namespace Calame
{
    public class DirtyMessage : IDirtyMessage
    {
        public IDocumentContext DocumentContext { get; }
        public IEnumerable DirtyObjects { get; }

        public DirtyMessage(IDocumentContext documentContext, object dirtyObject)
        {
            DocumentContext = documentContext;
            DirtyObjects = Enumerable<object>.New(dirtyObject);
        }

        public DirtyMessage(IDocumentContext documentContext, IEnumerable dirtyObjects)
        {
            DocumentContext = documentContext;
            DirtyObjects = dirtyObjects;
        }

        public DirtyMessage(IDocumentContext documentContext, params object[] dirtyObjects)
        {
            DocumentContext = documentContext;
            DirtyObjects = dirtyObjects;
        }
    }
}