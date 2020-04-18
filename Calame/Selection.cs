using System.Collections.Generic;
using System.Linq;
using Diese.Collections;

namespace Calame
{
    public class SelectionRequest<T> : SelectionMessageBase<T>, ISelectionRequest<T>
        where T : class
    {
        static public SelectionRequest<T> Empty(IDocumentContext documentContext) => new SelectionRequest<T>(documentContext);
        public ISelectionSpread<T> Promoted => new SelectionSpread<T>(DocumentContext, Items);

        private SelectionRequest(IDocumentContext documentContext)
            : base(documentContext)
        {
        }

        public SelectionRequest(IDocumentContext documentContext, T item)
            : base(documentContext, item)
        {
        }

        public SelectionRequest(IDocumentContext documentContext, IEnumerable<T> items)
            : base(documentContext, items)
        {
        }
    }

    public class SelectionSpread<T> : SelectionMessageBase<T>, ISelectionSpread<T>
        where T : class
    {
        static public SelectionSpread<T> Empty(IDocumentContext documentContext) => new SelectionSpread<T>(documentContext);

        private SelectionSpread(IDocumentContext documentContext)
            : base(documentContext)
        {
        }

        public SelectionSpread(IDocumentContext documentContext, T item)
            : base(documentContext, item)
        {
        }

        public SelectionSpread(IDocumentContext documentContext, IEnumerable<T> items)
            : base(documentContext, items)
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
                _item = null;
                _others = null;

                using (IEnumerator<T> enumerator = value.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                        return;

                    _item = enumerator.Current;
                    _others = enumerator.AsEnumerable().ToArray();
                }
            }
        }

        protected SelectionMessageBase(IDocumentContext documentContext)
        {
            DocumentContext = documentContext;
        }

        protected SelectionMessageBase(IDocumentContext documentContext, T item)
            : this(documentContext)
        {
            _item = item;
        }

        protected SelectionMessageBase(IDocumentContext documentContext, IEnumerable<T> items)
            : this(documentContext)
        {
            Items = items;
        }
    }
}