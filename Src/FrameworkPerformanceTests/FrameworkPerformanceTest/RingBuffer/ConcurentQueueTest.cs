using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.RingBuffer
{
    public class ConcurentQueueTest
    {
        private const int _powerTo = 1;
        private readonly int _queueSize = (int)Math.Pow(10, _powerTo);
        private readonly int _doubleQueueSize = (int)Math.Pow(10, _powerTo) * 2;
        private readonly FixedConcurrentQueue<Message> _fixedQueue;

        public ConcurentQueueTest()
        {
            _fixedQueue = new FixedConcurrentQueue<Message>(_queueSize);
        }

        [Benchmark(Description = "Concurrent Queue encode speed")]
        public void EnqeueRingQueueSpeedTest()
        {
            for (int i = 0; i < _queueSize; i++)
            {
                _fixedQueue.Enqueue(new Message { Value = i });
            }
        }

        [Benchmark(Description = "Concurrent Queue double encode speed")]
        public void EnqeueDoubleRingQueueSpeedTest()
        {
            for (int i = 0; i < _doubleQueueSize; i++)
            {
                _fixedQueue.Enqueue(new Message { Value = i });
            }
        }

        [Benchmark(Description = "Concurrent Queue encode/dequeue speed")]
        public void EnqeueDequeueRingQueueSpeedTest()
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

        [Benchmark(Description = "Concurrent Queue double encode/dequeue speed")]
        public void EnqeueDequeueDoubleRingQueueSpeedTest()
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

        [Benchmark(Description = "Concurrent Queue parallel enqueue / dequeue task")]
        public void ParallelEnqueueDequeueTest()
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
