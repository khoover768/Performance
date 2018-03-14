using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.BaseTypes
{
    public class TryPerformanceTests
    {
        [Benchmark(Description = "Test out return 'bool TryGet(string key, out string value)'")]
        public void TestOutReturn()
        {
            bool value = TryGet("key", out string stringValue);
        }

        [Benchmark(Description = "Test Tuple return '(bool Found, string Value) TryGet(string key)'")]
        public void TestTupleReturn()
        {
            (bool Found, string Value) restul = TryGet("key");
        }

        public bool TryGet(string key, out string value)
        {
            value = "33";
            return true;
        }

        public (bool Found, string Value) TryGet(string key)
        {
            return (true, "44");
        }
    }
}
