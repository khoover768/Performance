using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.RingBuffer
{
    public class FixedSizeQueue<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _maxSize;
        private readonly object _lock = new object();

        public FixedSizeQueue(int maxSize)
        {
            _queue = new Queue<T>(maxSize);
            _maxSize = maxSize;
        }

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item);
                if (_queue.Count > _maxSize)
                {
                    _queue.Dequeue();
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

        public bool TryDequeue(out T value)
        {
            value = default(T);

            lock (_lock)
            {
                if (_queue.Count == 0)
                {
                    return false;
                }

                value = _queue.Dequeue();
                return true;
            }
        }
    }
}
