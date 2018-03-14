using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest
{
    internal class Message
    {
        public int Value { get; set; }
    }

    internal struct MessageStruct
    {
        public MessageStruct(int value, long longValue)
        {
            Value = value;
            LongValue = longValue;
        }

        public int Value { get; }

        public long LongValue { get; }
    }
}
