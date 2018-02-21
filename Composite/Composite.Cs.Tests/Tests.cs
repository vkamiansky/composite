using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

using SimpleComposite = Composite.DataTypes.Composite<Composite.Cs.Tests.Simple>.Composite;

namespace Composite.Cs.Tests
{
    public class Tests
    {
        [Fact]
        public void ToBatchedTest()
        {
            var inputSequence = Enumerable.Range(1, 10)
                .Select(x => new Simple { Number = x, })
                .AsLimited(6);
            var batches = inputSequence.AsBatched(10, x => x.Number)
                .Take(2)
                .ToArray();

            Assert.Equal(1, batches[0][0].Number);
            Assert.Equal(2, batches[0][1].Number);
            Assert.Equal(3, batches[0][2].Number);
            Assert.Equal(4, batches[0][3].Number);
            Assert.Equal(5, batches[1][0].Number);
            Assert.Equal(6, batches[1][1].Number);
        }

        [Fact]
        public void ToPagedTest()
        {
            var inputSequence = Enumerable.Range(1, 10).AsLimited(6);
            var pages = inputSequence.AsPaged(2).Take(3).ToArray();

            Assert.Equal(1, pages[0][0]);
            Assert.Equal(2, pages[0][1]);
            Assert.Equal(3, pages[1][0]);
            Assert.Equal(4, pages[1][1]);
            Assert.Equal(5, pages[2][0]);
            Assert.Equal(6, pages[2][1]);
        }

        [Fact]
        public void ToPartitionedTest()
        {
            var inputSequence = Enumerable.Range(1, 10).AsLimited(6);
            var partitions = inputSequence.AsPartitioned(3);

            var arr1 = partitions[0].Take(2).ToArray();
            var arr2 = partitions[1].Take(2).ToArray();
            var arr3 = partitions[2].Take(2).ToArray();

            Assert.Equal(1, arr1[0]);
            Assert.Equal(2, arr1[1]);
            Assert.Equal(3, arr2[0]);
            Assert.Equal(4, arr2[1]);
            Assert.Equal(5, arr3[0]);
            Assert.Equal(6, arr3[1]);
        }

        [Fact]
        public void ToForestTest()
        {
            var obj = C.Composite(new[] {
                C.Value(new Simple { Number = 2, }),
                C.Composite(new [] {
                    C.Value(new Simple { Number = 1, }),
                    C.Value(new Simple { Number = 6, }),
                }),
                C.Value(new Simple { Number = 2, }),
            }.AsLimited(2));

            C.ToForest(obj).Take(2).ToArray();

            Assert.Throws<InvalidOperationException>(() =>
            {
                C.ToForest(obj).Take(3).ToArray();
            });
        }

        [Fact]
        public void ToFlatTest()
        {
            var obj = C.Composite(new[] {
                C.Value(new Simple { Number = 1, }),
                C.Composite(new [] {
                    C.Value(new Simple { Number = 2, }),
                    C.Value(new Simple { Number = 3, }),
                    C.Composite(new [] {
                        C.Value(new Simple { Number = 4, }),
                        C.Value(new Simple { Number = 5, }),
                    }.AsLimited(1)),
                }),
                C.Value(new Simple { Number = 6, }),
            }.AsLimited(2));

            var result = C.ToFlat(obj).Take(4).ToArray();
            for (int i = 1; i <= 4; i++)
            {
                Assert.Equal(i, result[i - 1].Number);
            }

            Assert.Throws<InvalidOperationException>(() =>
            {
                C.ToFlat(obj).Take(5).ToArray();
            });
        }

        [Fact]
        public void AnaTest()
        {
            var scn = new Func<Simple, Simple[]>[] {
                    x => x.Number == 1
                        ? new [] { new Simple { Number = 2, }, new Simple { Number = 3, }}
                        : new[] { x },
                    x => x.Number == 2
                        ? new [] { new Simple { Number = 4, }, new Simple { Number = 5, }}
                        : new[] { x },
                };

            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
            });

            var result = C.Ana(scn, obj);

            Assert.True(result.IsComposite);

            if (result is SimpleComposite compositeResult)
            {
                var resultArray = compositeResult.Item.ToArray();

                Assert.True(resultArray.Length == 2);
                Assert.True(resultArray[0].IsComposite);
                Assert.True(resultArray[1].IsValue);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}