using System.Collections.Generic;
using System.Linq;
using Diese.Collections;

namespace Calame
{
    public class SelectionRequest<T> : SelectionMessageBase<T>, ISelectionRequest<T>
        where T : class
    {
        static public SelectionRequest<T> Empty(IDocumentContext documentcontext) => new SelectionRequest<T>(documentcontext);
        public ISelectionSpread<T> Promoted => new SelectionSpread<T>(DocumentContext, Items);

        private SelectionRequest(IDocumentContext documentcontext)
            : base(documentcontext)
        {
        }

        public SelectionRequest(IDocumentContext documentcontext, T item)
            : base(documentcontext, item)
        {
        }

        public SelectionRequest(IDocumentContext documentcontext, IEnumerable<T> items)
            : base(documentcontext, items)
        {
        }
    }

    public class SelectionSpread<T> : SelectionMessageBase<T>, ISelectionSpread<T>
        where T : class
    {
        private SelectionSpread(IDocumentContext documentcontext)
            : base(documentcontext)
        {
        }

        public SelectionSpread(IDocumentContext documentcontext, T item)
            : base(documentcontext, item)
        {
        }

        public SelectionSpread(IDocumentContext documentcontext, IEnumerable<T> items)
            : base(documentcontext, items)
        {
        }
    }
    
    public class SelectionMessageBase<T> : ISelectionMessage<T>
        where T : class
    {
        private T _item;
        private T[] _others;
        
        public IDocumentContext DocumentContext { get; }

        public T Item
        {
            get => _item;
            set
            {
                _item = value;
                _others = null;
            }
        }

        public IEnumerable<T> Items
        {
            get
            {
                yield return Item;
                
                if (_others != null)
                    foreach (T other in _others)
                        yield return other;
            }
            set
            {
                using (IEnumerator<T> enumerator = value.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                        return;

                    _item = enumerator.Current;
                    _others = enumerator.AsEnumerable().ToArray();
                }
            }
        }

        protected SelectionMessageBase(IDocumentContext documentcontext)
        {
            DocumentContext = documentcontext;
        }

        protected SelectionMessageBase(IDocumentContext documentcontext, T item)
            : this(documentcontext)
        {
            _item = item;
        }

        protected SelectionMessageBase(IDocumentContext documentcontext, IEnumerable<T> items)
            : this(documentcontext)
        {
            Items = items;
        }
    }
}