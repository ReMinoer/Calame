using System.Collections.Generic;
using System.Linq;
using Diese.Collections;

namespace Calame
{
    static public class Selection
    {
        static public Selection<T> New<T>(T item)
        {
            return new Selection<T>(item);
        }
    }
    
    public class Selection<T> : ISelection<T>
    {
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