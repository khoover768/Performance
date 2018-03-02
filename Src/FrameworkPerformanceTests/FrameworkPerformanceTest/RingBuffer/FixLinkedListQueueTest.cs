using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.RingBuffer
{
    public class FixLinkedListQueueTest
    {
        private readonly int _queueSize = (int)Math.Pow(10, 6);
        private readonly int _doubleQueueSize = (int)Math.Pow(10, 6) * 2;
        private readonly FixedLinkedListQueue<Message> _fixedQueue;

        public FixLinkedListQueueTest()
        {
            _fixedQueue = new FixedLinkedListQueue<Message>(_queueSize);
        }

        [Benchmark(Description = "Linked Queue encode speed")]
        public void EnqeueLinkedListSpeedTest()
        {
            for (int i = 0; i < _queueSize; i++)
            {
                _fixedQueue.Enqueue(new Message { Value = i });
            }
        }

        [Benchmark(Description = "Linked Queue double encode speed")]
        public void EnqeueDoubleLinkedListSpeedTest()
        {
            for (int i = 0; i < _doubleQueueSize; i++)
            {
                _fixedQueue.Enqueue(new Message { Value = i });
            }
        }

        [Benchmark(Description = "Linked Queue encode/dequeue speed")]
        public void EnqeueDequeueLinkedListSpeedTest()
        {
            for (int i = 0; i < _queueSize; i++)
            {
                _fixedQueue.Enqueue(new Message { Value = i });
            }

            for (int i = 0; i < _queueSize; i++)
            {
                Message msg = _fixedQueue.Dequeue();
            }
        }

        [Benchmark(Description = "Linked Queue double encode/dequeue speed")]
        public void EnqeueDequeueDoubleLinkedListSpeedTest()
        {

            for (int i = 0; i < _doubleQueueSize; i++)
            {
                _fixedQueue.Enqueue(new Message { Value = i });
            }

            for (int i = 0; i < _queueSize; i++)
            {
                Message msg = _fixedQueue.Dequeue();
            }
        }

        [Benchmark(Description = "Linked Queue parallel enqueue / dequeue task")]
        public void ParallelEnqueueDequeueLinkedListTest()
        {
            Task t1 = Task.Run(() => EnqueueTask());
            Task t2 = Task.Run(() => DequeueTask());

            Task.WaitAll(t1, t2);
        }

        private void EnqueueTask()
        {
            for (int i = 0; i < _queueSize; i++)
            {
                _fixedQueue.Enqueue(new Message { Value = i });
            }
        }

        private void DequeueTask()
        {
            int count = 0;

            while (_fixedQueue.TryDequeue(out Message message))
            {
                count++;
                if (count == _queueSize)
                {
                    break;
                }
            }
        }
    }
}
