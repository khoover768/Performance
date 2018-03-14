using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest
{
    public static class Config
    {
        public static int QueueSize = (int)Math.Pow(10, 3);
        public static int DoubleQueueSize = (int)Math.Pow(10, 3) * 2;
    }
}
