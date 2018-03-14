using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.LruList
{
    public class FixedListStorageTests
    {
        public void FixedListStorageVariationTests()
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
                    Add = 5,
                    ShouldBeCount = 2,
                    DeletedCount = 3,
                },
                new {
                    Size = 10,
                    Add = 10,
                    ShouldBeCount = 10,
                    DeletedCount = 0,
                },
                new {
                    Size = 10,
                    Add = 10,
                    ShouldBeCount = 0,
                    DeletedCount = 10,
                },
                new {
                    Size = 100,
                    Add = 50,
                    ShouldBeCount = 50,
                    DeletedCount = 0,
                },
                new {
                    Size = 100,
                    Add = 100,
                    ShouldBeCount = 50,
                    DeletedCount = 50,
                },
            };

            foreach (var test in variations)
            {
                FixedListStorage<int> list = new FixedListStorage<int>(test.Size);

                var baseList = new List<int>();
                foreach (var index in Enumerable.Range(0, test.Add))
                {
                    list.Add(index);
                    baseList.Add(index);
                }

                var dataList = new List<int>(list);
                dataList.Count.Should().Be(test.Add);

                baseList
                    .Zip(dataList, (f, s) => new { f, s })
                    .All(x => x.f == x.s)
                    .Should().BeTrue();

                List<int> deletedList = null;
                if (test.DeletedCount > 0)
                {
                    deletedList = new List<int>(dataList.Shuffle().Take(test.DeletedCount));

                    foreach (var index in deletedList)
                    {
                        list.Remove(index);
                    }
                }

                list.Count.Should().Be(test.ShouldBeCount);
                list.RemovedCount.Should().Be(test.DeletedCount);

                if (deletedList != null)
                {
                    var deltaList = new List<int>(baseList.Except(deletedList).OrderBy(x => x));

                    list
                        .Zip(deltaList, (f, s) => new { f, s })
                        .All(x => x.f == x.s)
                        .Should().BeTrue();
                }
            }
        }
    }
}
