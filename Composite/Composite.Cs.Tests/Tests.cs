using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

using SimpleComposite = Composite.DataTypes.Composite<Composite.Cs.Tests.Tests.Simple>.Composite;

namespace Composite.Cs.Tests {

    public class Tests {

        public class Simple {
            public int Number { get; set; }
        }

        [Fact]
        public void ToPartitionedTest () {
            var inputSequence = Enumerable.Range(1, 10);
            var partitions = C.ToPartitioned(3, inputSequence);

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
        public void UnfoldTest () {
            var scn = new Func<Simple, Simple[]>[] {
                    x => x.Number == 1
                        ? new [] { new Simple { Number = 2, }, new Simple { Number = 3, }}
                        : new[] { x },
                    x => x.Number == 2
                        ? new [] { new Simple { Number = 4, }, new Simple { Number = 5, }}
                        : new[] { x },
                };

            var obj = C.Composite (new [] {
                new Simple { Number = 1, },
                new Simple { Number = 6, },
            });

            var result = C.Ana (scn, obj);

            Assert.True(result.IsComposite);

            if (result is SimpleComposite compositeResult) {
                var resultArray = compositeResult.Item.ToArray();

                Assert.True(resultArray.Length == 2);
                Assert.True(resultArray[0].IsComposite);
                Assert.True(resultArray[1].IsValue);
            }
            else {
                Assert.True(false);
            }
        }
    }
}