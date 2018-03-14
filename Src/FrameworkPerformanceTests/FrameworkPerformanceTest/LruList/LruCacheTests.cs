using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class LruCacheTests
    {
        private LruCache<string, CacheItem> _cache;

        public LruCacheTests()
        {
        }

        [Benchmark(Description = "LruCache encode speed")]
        public void SetSpeedTest()
        {
            _cache = new LruCache<string, CacheItem>(Config.QueueSize);

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Set(key, new CacheItem { Value = i });
            }
        }

        [Benchmark(Description = "LruCache double encode speed")]
        public void SetDoubleSpeedTest()
        {
            _cache = new LruCache<string, CacheItem>(Config.QueueSize);

            for (int i = 0; i < Config.DoubleQueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Set(key, new CacheItem { Value = i });
            }
        }

        [Benchmark(Description = "LruCache encode/Get speed")]
        public void SetGetSpeedTest()
        {
            _cache = new LruCache<string, CacheItem>(Config.QueueSize);

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

        [Benchmark(Description = "LruCache double encode/Get speed")]
        public void SetGetDoubleSpeedTest()
        {
            _cache = new LruCache<string, CacheItem>(Config.QueueSize);

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

        [Benchmark(Description = "Parallel LruCache enqueue / Get task")]
        public void ParallelEnqueueGetTest()
        {
            _cache = new LruCache<string, CacheItem>(Config.QueueSize);

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
            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.TryGetValue(key, out CacheItem value, true);
            }
        }
    }
}
