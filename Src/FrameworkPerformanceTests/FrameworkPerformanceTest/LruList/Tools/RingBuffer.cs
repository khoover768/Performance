using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class RingBuffer<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _maxSize;
        private readonly object _lock = new object();

        public RingBuffer(int maxSize)
        {
            _queue = new Queue<T>(maxSize);
            _maxSize = maxSize;
        }

        public RingBuffer(int maxSize, Action<T> onDump)
            : this(maxSize)
        {
            OnDump = onDump;
        }

        public RingBuffer(int maxSize, IEnumerable<T> values)
            : this(maxSize)
        {
            foreach (var item in values)
            {
                _queue.Enqueue(item);
            }
        }

        public RingBuffer(int maxSize, IEnumerable<T> values, Action<T> onDump)
            : this(maxSize, values)
        {
            OnDump = onDump;
        }

        public int Count { get { return _queue.Count; } }

        public Action<T> OnDump { get; }

        public void Clear()
        {
            lock (_lock)
            {
                _queue.Clear();
            }
        }

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item);

                if (_queue.Count > _maxSize)
                {
                    T rejectedValue = _queue.Dequeue();
                    OnDump?.Invoke(rejectedValue);
                }
            }
        }

        public T Dequeue()
        {
            lock (_lock)
            {
                return _queue.Dequeue();
            }
        }

        public (bool Found, T Value) Peek()
        {
            lock (_lock)
            {
                if (_queue.Count == 0)
                {
                    return (false, default(T));
                }

                return (true, _queue.Peek());
            }
        }

        public (bool Found, T Value) TryDequeue()
        {
            lock (_lock)
            {
                if (_queue.Count == 0)
                {
                    return (false, default(T));
                }

                return (true, _queue.Dequeue());
            }
        }
    }
}
