using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.RingBuffer
{
    public class RingQueueTests
    {
        private readonly int _queueSize = (int)Math.Pow(10, 6);
        private readonly int _doubleQueueSize = (int)Math.Pow(10, 6) * 2;
        private readonly RingQueue<Message> _ringQueue;

        public RingQueueTests()
        {
            _ringQueue = new RingQueue<Message>(_queueSize);
        }

        [Benchmark(Description = "Ring queue encode speed")]
        public void EnqeueRingQueueSpeedTest()
        {
            for (int i = 0; i < _queueSize; i++)
            {
                _ringQueue.Enqueue(new Message { Value = i });
            }
        }

        [Benchmark(Description = "Ring queue double encode speed")]
        public void EnqeueDoubleRingQueueSpeedTest()
        {
            for (int i = 0; i < _doubleQueueSize; i++)
            {
                _ringQueue.Enqueue(new Message { Value = i });
            }
        }

        [Benchmark(Description = "Ring queue encode/dequeue speed")]
        public void EnqeueDequeueRingQueueSpeedTest()
        {
            for (int i = 0; i < _queueSize; i++)
            {
                _ringQueue.Enqueue(new Message { Value = i });
            }

            for (int i = 0; i < _queueSize; i++)
            {
                Message msg = _ringQueue.Dequeue();
            }
        }

        [Benchmark(Description = "Ring queue double encode/dequeue speed")]
        public void EnqeueDequeueDoubleRingQueueSpeedTest()
        {

            for (int i = 0; i < _doubleQueueSize; i++)
            {
                _ringQueue.Enqueue(new Message { Value = i });
            }

            for (int i = 0; i < _queueSize; i++)
            {
                Message msg = _ringQueue.Dequeue();
            }
        }

        [Benchmark(Description = "Parallel enqueue / dequeue task")]
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
                _ringQueue.Enqueue(new Message { Value = i });
            }
        }

        private void DequeueTask()
        {
            int count = 0;

            while (_ringQueue.TryDequeue(out Message message))
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
