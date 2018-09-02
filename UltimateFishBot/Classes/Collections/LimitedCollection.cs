using System.Collections;
using System.Collections.Generic;

namespace UltimateFishBot.Collections
{
    public class LimitedCollection<T> : IEnumerable<T>
    {
        protected readonly T[] Storage;
        private int _current;

        public LimitedCollection(int size)
        {
            Storage = new T[size];
            _current = 0;
        }

        public void Add(T loc)
        {
            Storage[FindLeastImportant()] = loc;
        }

        protected virtual int FindLeastImportant()
        {
            if (Storage.Length <= _current)
            {
                _current = 0;
            }
            return _current++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = _current; i < Storage.Length; i++)
            {
                if (Storage[i] == null) break;

                yield return Storage[i];
            }

            for (var i = 0; i < _current; i++)
            {
                yield return Storage[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}