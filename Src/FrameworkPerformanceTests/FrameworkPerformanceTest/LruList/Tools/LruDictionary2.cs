using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class LruDictionary2<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly object _lock = new object();
        private readonly Dictionary<TKey, ValueItem> _dict;
        private readonly Action<TValue> _onRemoved;
        private int _taskLock = 0;

        public LruDictionary2(int maxSize)
        {
            MaxSize = maxSize;
            _dict = new Dictionary<TKey, ValueItem>();
        }

        public LruDictionary2(int maxSize, Action<TValue> onRemoved)
            : this(maxSize)
        {
            _onRemoved = onRemoved;
        }

        public LruDictionary2(int maxSize, IEqualityComparer<TKey> equalityComparer)
        {
            MaxSize = maxSize;
            _dict = new Dictionary<TKey, ValueItem>(equalityComparer);
        }

        public LruDictionary2(int maxSize, IEqualityComparer<TKey> equalityComparer, Action<TValue> onRemoved)
            : this(maxSize, equalityComparer)
        {
            _onRemoved = onRemoved;
        }

        /// <summary>
        /// Get or set value based on key.  If key does not exist, return default(T)
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>default(T) or value</returns>
        public TValue this[TKey key]
        {
            get
            {
                bool found = TryGetValue(key, out TValue value);
                if (found)
                {
                    return value;
                }

                return default(TValue);
            }
            set
            {
                Set(key, value);
            }
        }

        /// <summary>
        /// Capacity of LRU cache (cannot be changed)
        /// </summary>
        public int MaxSize { get; }

        /// <summary>
        /// Current count of cache
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _dict.Count;
                }
            }
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <returns>this</returns>
        public LruDictionary2<TKey, TValue> Clear()
        {
            lock (_lock)
            {
                _dict.Clear();
            }

            return this;
        }

        /// <summary>
        /// Get or create new item
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="create">function to create</param>
        /// <returns>retrieved or created value</returns>
        public TValue GetOrCreate(TKey key, Func<TValue> create)
        {
            lock (_lock)
            {
                if (_dict.TryGetValue(key, out ValueItem valueItem))
                {
                    _dict[key] = new ValueItem(valueItem.Value);
                    return valueItem.Value;
                }

                TValue value = create();
                _dict[key] = new ValueItem(value);

                if (_dict.Count >= MaxSize)
                {
                    Task.Run(() => GarbageCollector());
                }

                return value;
            }
        }

        /// <summary>
        /// Set cache item.  If value already exist, remove it and add the new one
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>this</returns>
        public LruDictionary2<TKey, TValue> Set(TKey key, TValue value)
        {
            lock (_lock)
            {
                _dict[key] = new ValueItem(value);
                return this;
            }
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true if removed, false if not</returns>
        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                return _dict.Remove(key);
            }
        }

        /// <summary>
        /// Try to get value from cache
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="promote">true to mark cache item used, false will not</param>
        /// <returns>true if found and value, false if not</returns>
        public bool TryGetValue(TKey key, out TValue value, bool promote = true)
        {
            lock (_lock)
            {
                ValueItem valueItem;
                if (!_dict.TryGetValue(key, out valueItem))
                {
                    value = default(TValue);
                    return false;
                }

                value = valueItem.Value;

                if (promote == false)
                {
                    return true;
                }

                _dict[key] = new ValueItem(valueItem.Value);
                return true;
            }
        }

        /// <summary>
        /// Try to remove key, set out value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value to set</param>
        /// <returns>true if removed and value, false if not</returns>
        public bool TryRemove(TKey key, out TValue value)
        {
            lock (_lock)
            {
                ValueItem valueItem;
                if (!_dict.TryGetValue(key, out valueItem))
                {
                    value = default(TValue);
                    return false;
                }

                value = valueItem.Value;
                return true;
            }
        }

        /// <summary>
        /// Remove the lest accessed items
        /// </summary>
        private void GarbageCollector()
        {
            int currentLock = Interlocked.CompareExchange(ref _taskLock, 1, 0);
            if (currentLock == 1)
            {
                return;
            }

            int takeCount = (int)((double)MaxSize * 0.1);

            try
            {
                var dateList = _dict
                    .OrderByDescending(x => x.Value.LastTimeAccessed)
                    .Take(takeCount)
                    .ToList();

                lock (_lock)
                {
                    for (int i = 0; i < dateList.Count; i++)
                    {
                        _dict.Remove(dateList[i].Key);
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref _taskLock, 0);
            }
        }

        /// <summary>
        /// Return enumerator of cache item details, from lease used to most used
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            lock (_lock)
            {
                return new List<TValue>(_dict.Values.Select(x => x.Value))
                    .ToList()
                    .GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnRemoved(KeyValuePair<TKey, TValue> kv)
        {
            _dict.Remove(kv.Key);
            _onRemoved?.Invoke(kv.Value);
        }

        private struct ValueItem
        {
            public ValueItem(TValue value)
            {
                Value = value;
                LastTimeAccessed = DateTime.Now;
            }

            public TValue Value { get; }

            public DateTime LastTimeAccessed { get; }
        }
    }
}
