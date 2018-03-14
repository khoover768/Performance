using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.BaseTypes
{
    public class LoopTests
    {
        private readonly int _max = (int)Math.Pow(10, 6);
        private readonly List<int> _array;

        public LoopTests()
        {
            _array = new List<int>(Enumerable.Range(0, _max));
        }

        [Benchmark(Description = "Iterate for(...)")]
        public void ForLoopTest()
        {
            int total = 0;
            for (int i = 0; i < _max; i++)
            {
                total += _array[i];
            }
        }

        [Benchmark(Description = "Iterate foreach(...)")]
        public void ForEachLoopTest()
        {
            int total = 0;
            foreach(var item in _array)
            {
                total += item;
            }
        }

        [Benchmark(Description = "Iterate foreach(... with Enumerable.Range)")]
        public void ForEachWithRangeLoopTest()
        {
            int total = 0;
            foreach (var index in Enumerable.Range(0, _max))
            {
                total += _array[index];
            }
        }
    }
}
