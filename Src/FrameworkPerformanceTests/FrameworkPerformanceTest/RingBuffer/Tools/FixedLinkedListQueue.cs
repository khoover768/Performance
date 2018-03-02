using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.RingBuffer
{
    public class FixedLinkedListQueue<T>
    {
        private LinkedList<T> _linkedList = new LinkedList<T>();
        private readonly int _maxSize;
        private readonly object _lock = new object();

        public FixedLinkedListQueue(int maxSize)
        {
            _maxSize = maxSize;
        }

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                _linkedList.AddLast(new LinkedListNode<T>(item));

                if (_linkedList.Count > _maxSize)
                {
                    _linkedList.RemoveFirst();
                }
            }
        }

        public T Dequeue()
        {
            lock (_lock)
            {
                if (_linkedList.Count == 0)
                {
                    throw new Exception("Empty list");
                }

                T item = _linkedList.First();
                _linkedList.RemoveFirst();
                return item;
            }
        }

        public bool TryDequeue(out T value)
        {
            value = default(T);

            lock (_lock)
            {
                if (_linkedList.Count == 0)
                {
                    return false;
                }

                value = _linkedList.First();
                _linkedList.RemoveFirst();
                return true;
            }
        }
    }
}
