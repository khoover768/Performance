using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class LruDictionary2PerformanceTests
    {
        private LruDictionary2<string, CacheItem> _cache;

        public LruDictionary2PerformanceTests()
        {
        }

        [Benchmark(Description = "LruDictionary2 encode speed")]
        public void SetSpeedTest()
        {
            _cache = new LruDictionary2<string, CacheItem>(Config.QueueSize);

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Set(key, new CacheItem { Value = i });
            }
        }

        [Benchmark(Description = "LruDictionary2 double encode speed")]
        public void SetDoubleSpeedTest()
        {
            _cache = new LruDictionary2<string, CacheItem>(Config.QueueSize);

            for (int i = 0; i < Config.DoubleQueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Set(key, new CacheItem { Value = i });
            }
        }

        [Benchmark(Description = "LruDictionary2 encode/Get speed")]
        public void SetGetSpeedTest()
        {
            _cache = new LruDictionary2<string, CacheItem>(Config.QueueSize);

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Set(key, new CacheItem { Value = i });
            }

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                CacheItem msg = _cache[key];
            }
        }

        [Benchmark(Description = "LruDictionary2 double encode/Get speed")]
        public void SetGetDoubleSpeedTest()
        {
            _cache = new LruDictionary2<string, CacheItem>(Config.QueueSize);

            for (int i = 0; i < Config.DoubleQueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Set(key, new CacheItem { Value = i });
            }

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                CacheItem msg = _cache[key];
            }
        }

        [Benchmark(Description = "Parallel LruDictionary2 enqueue / Get task")]
        public void ParallelEnqueueGetTest()
        {
            _cache = new LruDictionary2<string, CacheItem>(Config.QueueSize);

            Task t1 = Task.Run(() => EnqueueTask());
            Task t2 = Task.Run(() => GetTask());

            Task.WaitAll(t1, t2);
        }

        private void EnqueueTask()
        {
            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Set(key, new CacheItem { Value = i });
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

                string key = $"Key_{index}";
                if (!_cache.TryGetValue(key, out CacheItem cacheItem, true))
                {
                    throw new InvalidOperationException("Try get failed");
                }

                index++;
            }
        }
    }
}
