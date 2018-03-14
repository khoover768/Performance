using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public static class ListExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> self)
        {
            var provider = new RNGCryptoServiceProvider();
            var list = new List<T>(self);
            int n = list.Count;

            while (n > 1)
            {
                var box = new byte[1];

                do
                {
                    provider.GetBytes(box);
                }
                while (!(box[0] < n * (Byte.MaxValue / n)));

                var k = (box[0] % n);
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
