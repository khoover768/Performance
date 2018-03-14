using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class LruListUnitTests
    {
        private readonly int _queueSize = (int)Math.Pow(10, 6);

        public void SizeTests()
        {
            var variations = new[]
            {
                new {
                    Size = 10,
                    Add = 0,
                    ShouldBeCount = 0,
                    DeletedCount = 0,
                },
                new {
                    Size = 10,
                    Add = 5,
                    ShouldBeCount = 5,
                    DeletedCount = 0,
                },
                new {
                    Size = 10,
                    Add = 10,
                    ShouldBeCount = 10,
                    DeletedCount = 0,
                },
                new {
                    Size = 10,
                    Add = 11,
                    ShouldBeCount = 10,
                    DeletedCount = 1,
                },
                new {
                    Size = 10,
                    Add = 15,
                    ShouldBeCount = 10,
                    DeletedCount = 5,
                },
                new {
                    Size = 20,
                    Add = 55,
                    ShouldBeCount = 20,
                    DeletedCount = 55 - 20,
                },
                new {
                    Size = 100,
                    Add = 500,
                    ShouldBeCount = 100,
                    DeletedCount = 500 - 100,
                },
            };

            foreach (var test in variations)
            {
                LruList<CacheItem> list = new LruList<CacheItem>(test.Size);

                foreach (var index in Enumerable.Range(0, test.Add))
                {
                    list.Add(new CacheItem(index));
                }

                list.Count.Should().Be(test.ShouldBeCount);
                list.DumpCount.Should().Be(test.DeletedCount);

                var itemList = new List<CacheItem>(list.OrderBy(x => x.Value));
                itemList.Count.Should().Be(test.ShouldBeCount);

                int start = Math.Max(test.Add - test.Size, 0);
                int size = Math.Min(test.Add, test.Size);
                itemList.Zip(Enumerable.Range(start, size), (f, s) => new { f = f, s = s })
                    .All(x => x.f.Value == x.s)
                    .Should().BeTrue();
            }
        }

        public void FuzzyTest()
        {
            var rnd = new Random();

            var variations = new[]
            {
                new {
                    Size = 10,
                    Add = 0,
                    ShouldBeCount = 0,
                    DeletedCount = 0,
                },
                new {
                    Size = 10,
                    Add = 5,
                    ShouldBeCount = 5,
                    DeletedCount = 0,
                },
                new {
                    Size = 10,
                    Add = 10,
                    ShouldBeCount = 10,
                    DeletedCount = 0,
                },
                new {
                    Size = 10,
                    Add = 11,
                    ShouldBeCount = 10,
                    DeletedCount = 1,
                },
                new {
                    Size = 10,
                    Add = 15,
                    ShouldBeCount = 10,
                    DeletedCount = 5,
                },
                new {
                    Size = 20,
                    Add = 55,
                    ShouldBeCount = 20,
                    DeletedCount = 55 - 20,
                },
                new {
                    Size = 100,
                    Add = 500,
                    ShouldBeCount = 100,
                    DeletedCount = 500 - 100,
                },
            };

            foreach (var test in variations)
            {
                LruList<CacheItem> list = new LruList<CacheItem>(test.Size);

                var inputList = new List<int>(Enumerable.Range(0, test.Add).Select(x => rnd.Next(0, Math.Min(test.Add, test.Add / 4))));

                foreach (var index in inputList)
                {
                    list.Add(new CacheItem(index));
                }

                list.Count.Should().Be(test.ShouldBeCount);
                list.DumpCount.Should().Be(test.DeletedCount);

                var itemList = new List<CacheItem>(list.OrderBy(x => x.Value));
                itemList.Count.Should().Be(test.ShouldBeCount);

                inputList
                    .Skip(test.DeletedCount)
                    .OrderBy(x => x)
                    .Zip(itemList, (f, s) => new { f, s })
                    .All(x => x.f == x.s.Value)
                    .Should().BeTrue();
            }
        }

        private class CacheItem : IDisposable
        {
            public CacheItem(int value)
            {
                Value = value;
            }

            public int Value { get; }

            public void Dispose()
            {
            }
        }

        private struct CacheItemStruct
        {
            public CacheItemStruct(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}
