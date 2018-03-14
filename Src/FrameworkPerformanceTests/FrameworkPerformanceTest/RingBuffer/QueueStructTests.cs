using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.RingBuffer
{
    public class QueueStructTests
    {
        private readonly int _queueSize = (int)Math.Pow(10, 6);
        private readonly int _doubleQueueSize = (int)Math.Pow(10, 6) * 2;
        private readonly FixedSizeQueue<MessageStruct> _fixedQueueStruct;
        private const long _longValue = 100;

        public QueueStructTests()
        {
            _fixedQueueStruct = new FixedSizeQueue<MessageStruct>(_queueSize);
        }

        [Benchmark(Description = "Queue encode speed - struct")]
        public void EnqeueQueueSpeedTest()
        {
            for (int i = 0; i < _queueSize; i++)
            {
                _fixedQueueStruct.Enqueue(new MessageStruct(i, _longValue));
            }
        }

        [Benchmark(Description = "Queue double encode speed - struct")]
        public void EnqeueDoubleQueueSpeedTest()
        {
            for (int i = 0; i < _doubleQueueSize; i++)
            {
                _fixedQueueStruct.Enqueue(new MessageStruct(i, _longValue));
            }
        }

        [Benchmark(Description = "Queue encode/dequeue speed - struct")]
        public void EnqeueDequeueQueueSpeedTest()
        {
            for (int i = 0; i < _queueSize; i++)
            {
                _fixedQueueStruct.Enqueue(new MessageStruct(i, _longValue));
            }

            for (int i = 0; i < _queueSize; i++)
            {
                MessageStruct msg = _fixedQueueStruct.Dequeue();
            }
        }

        [Benchmark(Description = "Queue double encode/dequeue speed - struct")]
        public void EnqeueDequeueDoubleQueueSpeedTest()
        {

            for (int i = 0; i < _doubleQueueSize; i++)
            {
                _fixedQueueStruct.Enqueue(new MessageStruct(i, _longValue));
            }

            for (int i = 0; i < _queueSize; i++)
            {
                MessageStruct msg = _fixedQueueStruct.Dequeue();
            }
        }

        [Benchmark(Description = "Queue parallel enqueue / dequeue task - struct")]
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
                _fixedQueueStruct.Enqueue(new MessageStruct(i, _longValue));
            }
        }

        private void DequeueTask()
        {
            int count = 0;

            while (_fixedQueueStruct.TryDequeue(out MessageStruct message))
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
