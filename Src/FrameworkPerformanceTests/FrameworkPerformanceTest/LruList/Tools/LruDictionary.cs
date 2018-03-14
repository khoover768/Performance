using FrameworkPerformanceTest.LruList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class LruDictionary<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly object _lock = new object();
        private readonly Dictionary<TKey, ValueItem> _dict;
        private readonly LruList<KeyValuePair<TKey, TValue>> _lruList;
        private readonly Action<TValue> _onRemoved;

        public LruDictionary(int maxSize)
        {
            MaxSize = maxSize;
            _dict = new Dictionary<TKey, ValueItem>();
            _lruList = new LruList<KeyValuePair<TKey, TValue>>(MaxSize, OnRemoved);
        }

        public LruDictionary(int maxSize, Action<TValue> onRemoved)
            : this(maxSize)
        {
            _onRemoved = onRemoved;
        }

        public LruDictionary(int maxSize, IEqualityComparer<TKey> equalityComparer)
        {
            MaxSize = maxSize;
            _dict = new Dictionary<TKey, ValueItem>(equalityComparer);
            _lruList = new LruList<KeyValuePair<TKey, TValue>>(MaxSize, OnRemoved);
        }

        public LruDictionary(int maxSize, IEqualityComparer<TKey> equalityComparer, Action<TValue> onRemoved)
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
                if( found)
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
                    if (_lruList.Count != _dict.Count)
                    {
                        throw new InvalidOperationException("LRU list and dictionary count does not match");
                    }

                    return _dict.Count;
                }
            }
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <returns>this</returns>
        public LruDictionary<TKey, TValue> Clear()
        {
            lock (_lock)
            {
                _dict.Clear();
                _lruList.Clear();
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
                ValueItem valueItem;
                if (_dict.TryGetValue(key, out valueItem))
                {
                    _lruList.Promote(valueItem.Handle);
                    return valueItem.Value;
                }

                TValue value = create();
                LruHandle handle = _lruList.Add(new KeyValuePair<TKey, TValue>(key, value));
                _dict[key] = new ValueItem(value, handle);
                //_dict[key] = new ValueItem(value, new LruHandle(0));

                return value;
            }
        }

        /// <summary>
        /// Set cache item.  If value already exist, remove it and add the new one
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>this</returns>
        public LruDictionary<TKey, TValue> Set(TKey key, TValue value)
        {
            lock (_lock)
            {
                ValueItem valueItem;
                if (_dict.TryGetValue(key, out valueItem))
                {
                    _lruList.Promote(valueItem.Handle, new KeyValuePair<TKey, TValue>(key, value));
                    return this;
                }

                LruHandle handle = _lruList.Add(new KeyValuePair<TKey, TValue>(key, value));
                _dict[key] = new ValueItem(value, handle);
                //_dict[key] = new ValueItem(value, new LruHandle(0));

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
                ValueItem valueItem;
                if (!_dict.TryGetValue(key, out valueItem))
                {
                    return false;
                }

                _dict.Remove(key);
                _lruList.Remove(valueItem.Handle);
                return true;
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

                _lruList.Promote(valueItem.Handle);
                value = valueItem.Value;
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

                _dict.Remove(key);
                _lruList.Remove(valueItem.Handle);
                value = valueItem.Value;
                return true;
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
            public ValueItem(TValue value, LruHandle handle)
            {
                Value = value;
                Handle = handle;
            }

            public TValue Value { get; }

            public LruHandle Handle { get; }
        }
    }
}
