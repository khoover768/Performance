using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.RingBuffer
{
    public class FixedConcurrentQueue<T>
    {
        private ConcurrentQueue<T> _queue;
        private readonly int _maxSize;

        public FixedConcurrentQueue(int maxSize)
        {
            _queue = new ConcurrentQueue<T>();
            _maxSize = maxSize;
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        public T Dequeue()
        {
            if (!_queue.TryDequeue(out T result))
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        public bool TryDequeue(out T value)
        {
            value = default(T);

            if (!_queue.TryDequeue(out value))
            {
                return false;
            }

            return true;
        }
    }
}
