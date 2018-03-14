using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public struct LruHandle
    {
        public LruHandle(int index)
        {
            Index = index;
        }

        public int Index { get; }
    }
}
