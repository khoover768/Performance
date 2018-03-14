using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    /// <summary>
    /// GC List - manage list with GC for LRU enforcement to maintain max size
    /// 
    /// GC uses GcHandle(s) which are index into the master list, 0 offset for fast performance
    /// Gen 1 ring buffer is used to track LRU activities
    /// 
    /// </summary>
    /// <typeparam name="T">type to manage</typeparam>
    public class LruList<T> : IEnumerable<T>
    {
        private long _counter = 0;
        private int _pruneLock = 0;
        private readonly int _maxSize;
        private readonly RingBuffer<ItemReference> _gen1;
        private readonly RingBuffer<ItemReference> _gen2;
        private readonly RingBuffer<ItemDetail> _removeList;
        private readonly FixedListStorage<ItemDetail> _masterList;
        private long _dumpCount = 0;

        public LruList(int maxSize)
        {
            _maxSize = maxSize;

            int gen1Size = Math.Max(_maxSize, _maxSize / 4);
            _gen1 = new RingBuffer<ItemReference>(gen1Size, OnDumpFromGen1);

            _gen2 = new RingBuffer<ItemReference>(_maxSize);
            _masterList = new FixedListStorage<ItemDetail>(_maxSize);
            _removeList = new RingBuffer<ItemDetail>(_maxSize);
        }

        public LruList(int maxSize, Action<T> onRemoved)
            : this(maxSize)
        {
            OnRemoved = onRemoved;
        }

        /// <summary>
        /// Return count of active items in list
        /// </summary>
        public int Count { get { return _masterList.Count; } }

        /// <summary>
        /// Number of records dumped.
        /// </summary>
        public long DumpCount { get { return _dumpCount; } }

        /// <summary>
        /// Delegate for when item is removed
        /// </summary>
        public Action<T> OnRemoved { get; }

        /// <summary>
        /// Clear list
        /// </summary>
        public void Clear()
        {
            _gen1.Clear();
            _gen2.Clear();
            _removeList.Clear();
            _masterList.Clear();
        }

        /// <summary>
        /// Add value to GC list
        /// </summary>
        /// <param name="value">value (must be not null)</param>
        /// <returns>GC handle</returns>
        public LruHandle Add(T value)
        {
            long counter = Interlocked.Increment(ref _counter);

            Prune();

            int index = _masterList.Add(new ItemDetail(value, counter));

            var lruHandle = new LruHandle(index);
            _gen1.Enqueue(new ItemReference(lruHandle, counter));

            return lruHandle;
        }

        /// <summary>
        /// Promote a GC object for LRU
        /// </summary>
        /// <param name="handle">list handle returned by add</param>
        /// <returns>this</returns>
        public LruList<T> Promote(LruHandle handle)
        {
            long counter = Interlocked.Increment(ref _counter);

            _masterList.Update(handle.Index, x => new ItemDetail(x, counter));
            _gen1.Enqueue(new ItemReference(handle, counter));

            return this;
        }

        /// <summary>
        /// Promote a GC object for LRU
        /// </summary>
        /// <param name="handle">list handle returned by add</param>
        /// <param name="value">value to replace</param>
        /// <returns>this</returns>
        public LruList<T> Promote(LruHandle handle, T value)
        {
            long counter = Interlocked.Increment(ref _counter);

            _masterList.Update(handle.Index, x => new ItemDetail(value, counter));
            _gen1.Enqueue(new ItemReference(handle, counter));

            return this;
        }

        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="handle">handle of LRU</param>
        /// <returns>this</returns>
        public LruList<T> Remove(LruHandle handle)
        {
            _masterList.Remove(handle.Index);
            return this;
        }

        /// <summary>
        /// Prune _gen2 and then _gen1 queues
        /// </summary>
        private void Prune()
        {
            int pruneLock = Interlocked.CompareExchange(ref _pruneLock, 1, 0);
            if (pruneLock == 1)
            {
                return;
            }

            try
            {
                while (Count >= _maxSize)
                {
                    (bool Found, ItemReference value) result = _gen2.TryDequeue();
                    if (!result.Found)
                    {
                        result = _gen1.TryDequeue();
                    }

                    if (!result.Found)
                    {
                        return;
                    }

                    ItemReference itemToDump = result.value;

                    int index = itemToDump.Handle.Index;
                    bool found = _masterList.TryGetValue(index, out ItemDetail value);
                    if (!found || value.Counter != itemToDump.Counter)
                    {
                        continue;
                    }

                    _masterList.Remove(index);
                    _removeList.Enqueue(value);
                }

                while (true)
                {
                    (bool Found, ItemDetail Value) item = _removeList.TryDequeue();
                    if (!item.Found)
                    {
                        return;
                    }

                    Interlocked.Increment(ref _dumpCount);
                    OnRemoved?.Invoke(item.Value.Value);
                }
            }
            finally
            {
                Interlocked.CompareExchange(ref _pruneLock, 0, 1);
            }
        }

        /// <summary>
        /// Return enumerator for T
        /// </summary>
        /// <exception cref="InvalidOperationException">If list gets modified</exception>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            long currentCounter = _counter;
            var list = new List<ItemDetail>();

            foreach (var item in _masterList)
            {
                if (currentCounter != _counter)
                {
                    throw new InvalidOperationException("Collection changed while enumerating");
                }

                list.Add(item);
            }

            return list
                .OrderByDescending(x => x.Counter)
                .Select(x => x.Value)
                .ToList()
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Move to 2nd generation list
        /// </summary>
        /// <param name="index"></param>
        private void OnDumpFromGen1(ItemReference itemReference)
        {
            _gen2.Enqueue(itemReference);
        }

        /// <summary>
        /// Structure for keeping track of GC items
        /// </summary>
        [DebuggerDisplay("Counter={Counter}, Value={Value}")]
        private struct ItemDetail
        {
            public ItemDetail(ItemDetail valueItem, long counter)
            {
                Value = valueItem.Value;
                Counter = counter;
            }

            public ItemDetail(T value, long counter)
            {
                Value = value;
                Counter = counter;
            }

            public T Value { get; }

            /// <summary>
            /// Positive >= 0 for normal counter, negative number + 1 for index offsets
            /// </summary>
            public long Counter { get; }

            /// <summary>
            /// Set counter to new value, keep the value reference
            /// </summary>
            /// <param name="counter">counter</param>
            /// <returns>new value item</returns>
            public ItemDetail SetCounter(long counter)
            {
                return new ItemDetail(Value, counter);
            }
        }

        [DebuggerDisplay("Index={Handle.Index}, Counter={Counter}")]
        private struct ItemReference
        {
            public ItemReference(LruHandle handle, long counter)
            {
                Handle = handle;
                Counter = counter;
            }

            public LruHandle Handle { get; }

            public long Counter { get; }
        }
    }
}
