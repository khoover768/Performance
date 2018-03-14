using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class FixedListStorage<T> : IEnumerable<T>
    {
        private long _counter = 0;
        private object _lock = new object();
        private readonly int _maxSize;
        private readonly List<ValueItem> _masterList;
        private readonly Queue<int> _removedQueue;

        public FixedListStorage(int maxSize)
        {
            _maxSize = maxSize;
            _masterList = new List<ValueItem>(_maxSize);
            _removedQueue = new Queue<int>();
        }

        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    ValueItem valueItem = _masterList[index];

                    if (!valueItem.IsActive)
                    {
                        throw new InvalidOperationException("Cannot update a deleted index");
                    }

                    return valueItem.Value;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _masterList.Count - _removedQueue.Count;
                }
            }
        }

        public int RemovedCount { get { return _removedQueue.Count; } }

        public void Clear()
        {
            lock (_lock)
            {
                _masterList.Clear();
                _removedQueue.Clear();
            }
        }

        public int Add(T value)
        {
            lock (_lock)
            {
                int index = Next();

                if (index == _masterList.Count)
                {
                    _masterList.Add(new ValueItem(value));
                }
                else
                {
                    _masterList[index] = new ValueItem(value);
                }

                return index;
            }
        }

        public void Remove(int index)
        {
            lock (_lock)
            {
                _removedQueue.Enqueue(index);
                _masterList[index] = ValueItem.Deleted;
            }
        }

        public void Update(int index, Func<T, T> update)
        {
            lock (_lock)
            {
                ValueItem valueItem = _masterList[index];

                if (!valueItem.IsActive)
                {
                    throw new InvalidOperationException("Cannot update a deleted index");
                }

                _masterList[index] = new ValueItem(update(valueItem.Value));
            }
        }

        public bool TryGetValue(int index, out T Value)
        {
            Value = default(T);

            ValueItem valueItem = _masterList[index];
            if (!valueItem.IsActive)
            {
                return false;
            }

            Value = valueItem.Value;
            return true;
        }

        private int Next()
        {
            if (_removedQueue.Count != 0)
            {
                return _removedQueue.Dequeue();
            }

            if (_masterList.Count >= _maxSize)
            {
                throw new InvalidOperationException("Storage is full");
            }

            return _masterList.Count;
        }

        /// <summary>
        /// Return enumerator for T
        /// </summary>
        /// <exception cref="InvalidOperationException">If list gets modified</exception>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            long currentCounter = _counter;

            var list = new List<T>();

            lock (_lock)
            {
                for (int index = 0; index < _masterList.Count; index++)
                {
                    if (currentCounter != _counter)
                    {
                        throw new InvalidOperationException("Collection changed while enumerating");
                    }

                    // This is a table scan
                    if (_removedQueue.Contains(index))
                    {
                        continue;
                    }

                    list.Add(_masterList[index].Value);
                }
            }

            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private struct ValueItem
        {
            public ValueItem(T value)
            {
                Value = value;
                IsActive = true;
            }

            public T Value { get; }

            public bool IsActive { get; }

            public static ValueItem Deleted { get; } = new ValueItem();
        }
    }
}
