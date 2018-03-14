using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList.Tests
{
    public class LruUnitDictionaryTests
    {
        private List<int> _inputList;
        private static readonly int _size = (int)Math.Pow(10, 6);
        private static readonly Random _rnd = new Random();

        public LruUnitDictionaryTests()
        {
            _inputList = new List<int>(Enumerable.Range(0, _size).Select(x => _rnd.Next(0, Math.Min(_size, _size / 4))));
        }

        public void SimpleTests()
        {
            int dumpCount = 0;
            int foundCount = 0;
            int capacity = (int)Math.Pow(10, 2);
            var dict = new LruDictionary<int, Data>(capacity, x => dumpCount++);

            for (int i = 0; i < _inputList.Count; i++)
            {
                dict.Set(_inputList[i], new Data(_inputList[i]));
            }

            for (int i = _inputList.Count - 1; i >= 0; i--)
            {
                bool found = dict.TryGetValue(_inputList[i], out Data value);
                if (found)
                {
                    foundCount++;
                }
            }

            dict.Count.Should().Be(capacity);
            dumpCount.Should().BeGreaterThan(0);
            foundCount.Should().BeGreaterThan(0);
        }


        private struct Data
        {
            public Data(int key)
            {
                Key = key;
                Value = key;
            }

            public int Key { get; }

            public int Value { get; }
        }
    }
}
