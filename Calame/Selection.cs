using System.Collections.Generic;
using System.Linq;
using Diese.Collections;

namespace Calame
{
    static public class Selection
    {
        static public Selection<T> Empty<T>()
            where T : class
        {
            return Selection<T>.Empty;
        }

        static public Selection<T> Of<T>(T item)
            where T : class
        {
            return new Selection<T>(item);
        }
        
        static public Selection<T> Of<T>(params T[] items)
            where T : class
        {
            return new Selection<T>(items);
        }

        static public Selection<T> Of<T>(IEnumerable<T> items)
            where T : class
        {
            return new Selection<T>(items);
        }
    }
    
    public class Selection<T> : ISelection<T>
        where T : class
    {
        static public Selection<T> Empty => new Selection<T>();
        
        private T _item;
        private T[] _others;

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

        private Selection()
        {
            _item = null;
        }

        public Selection(T item)
        {
            _item = item;
        }

        public Selection(IEnumerable<T> items)
        {
            Items = items;
        }

        public Selection(params T[] items)
        {
            _item = items[0];
            _others = items.Skip(1).ToArray();
        }
    }
}