using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class DictionaryOnlyPerformanceTests
    {
        private Dictionary<string, CacheItem> _cache;
        private object _lock = new object();

        public DictionaryOnlyPerformanceTests()
        {
        }

        [Benchmark(Description = "DictionaryOnly encode speed")]
        public void SetSpeedTest()
        {
            _cache = new Dictionary<string, CacheItem>();

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Add(key, new CacheItem { Value = i });
            }
        }

        [Benchmark(Description = "DictionaryOnly double encode speed")]
        public void SetDoubleSpeedTest()
        {
            _cache = new Dictionary<string, CacheItem>();

            for (int i = 0; i < Config.DoubleQueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Add(key, new CacheItem { Value = i });
            }
        }

        [Benchmark(Description = "DictionaryOnly encode/Get speed")]
        public void SetGetSpeedTest()
        {
            _cache = new Dictionary<string, CacheItem>();

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Add(key, new CacheItem { Value = i });
            }

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                CacheItem msg = _cache[key];
            }
        }

        [Benchmark(Description = "DictionaryOnly double encode/Get speed")]
        public void SetGetDoubleSpeedTest()
        {
            _cache = new Dictionary<string, CacheItem>();

            for (int i = 0; i < Config.DoubleQueueSize; i++)
            {
                string key = $"Key_{i}";
                _cache.Add(key, new CacheItem { Value = i });
            }

            for (int i = 0; i < Config.QueueSize; i++)
            {
                string key = $"Key_{i}";
                CacheItem msg = _cache[key];
            }
        }

        [Benchmark(Description = "Parallel DictionaryOnly enqueue / Get task")]
        public void ParallelEnqueueGetTest()
        {
            _cache = new Dictionary<string, CacheItem>();

            Task t1 = Task.Run(() => EnqueueTask());
            Task t2 = Task.Run(() => GetTask());

            Task.WaitAll(t1, t2);
        }

        private void EnqueueTask()
        {
            for (int i = 0; i < Config.QueueSize; i++)
            {
                lock (_lock)
                {
                    string key = $"Key_{i}";
                    _cache.Add(key, new CacheItem { Value = i });
                }
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

                lock (_lock)
                {
                    string key = $"Key_{index}";
                    if (!_cache.TryGetValue(key, out CacheItem cacheItem))
                    {
                        throw new InvalidOperationException("Try get failed");
                    }
                }

                index++;
            }
        }
    }
}
