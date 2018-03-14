using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class FixedListStoragePerformanceTests
    {
        private FixedListStorage<CacheItem> _cache;

        public FixedListStoragePerformanceTests()
        {
        }

        [Benchmark(Description = "FixedListStorage set")]
        public void SetSpeedTest()
        {
            _cache = new FixedListStorage<CacheItem>(Config.QueueSize);

            for (int i = 0; i < Config.QueueSize; i++)
            {
                _cache.Add(new CacheItem { Value = i });
            }
        }

        [Benchmark(Description = "FixedListStorage set/get/remove")]
        public void SetGetRemoveSpeedTest()
        {
            _cache = new FixedListStorage<CacheItem>(Config.QueueSize);

            for (int i = 0; i < Config.QueueSize; i++)
            {
                _cache.Add(new CacheItem { Value = i });
            }

            for (int i = 0; i < Config.QueueSize; i++)
            {
                CacheItem msg = _cache[i];
            }

            for (int i = 0; i < Config.QueueSize; i++)
            {
                _cache.Remove(i);
            }
        }

        [Benchmark(Description = "Parallel FixedListStorage enqueue / Get task")]
        public void ParallelEnqueueGetTest()
        {
            _cache = new FixedListStorage<CacheItem>(Config.QueueSize);

            Task t1 = Task.Run(() => EnqueueTask());
            Task t2 = Task.Run(() => GetTask());

            Task.WaitAll(t1, t2);
        }

        private void EnqueueTask()
        {
            for (int i = 0; i < Config.QueueSize; i++)
            {
                _cache.Add(new CacheItem { Value = i });
            }
        }

        private void GetTask()
        {
            int index = 0;
            while (index < Config.QueueSize)
            {
                if (index >= _cache.Count)
                {
                    continue;
                }

                if (!_cache.TryGetValue(index, out CacheItem value))
                {
                    throw new InvalidOperationException("Try get failed");
                }

                index++;
            }
        }
    }
}
