using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

using SimpleComposite = Composite.DataTypes.Composite<Composite.Cs.Tests.Tests.Simple>.Composite;

namespace Composite.Cs.Tests {
    public class Tests {

        internal class Simple {
            public int Number { get; set; }
        }

        [Fact]
        public void ToPagedTest () {
            var inputSequence = Enumerable.Range(1, 10).AsLimited(6);
            var pages = C.ToPaged(2, inputSequence).Take(3).ToArray();

            Assert.Equal(1, pages[0][0]);
            Assert.Equal(2, pages[0][1]);
            Assert.Equal(3, pages[1][0]);
            Assert.Equal(4, pages[1][1]);
            Assert.Equal(5, pages[2][0]);
            Assert.Equal(6, pages[2][1]);
        }

        [Fact]
        public void ToPartitionedTest () {
            var inputSequence = Enumerable.Range(1, 10).AsLimited(6);
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
        public void ToForestTest(){
            var obj = C.Composite (new [] {
                C.Value(new Simple { Number = 2, }),
                C.Composite(new [] {
                    C.Value( new Simple { Number = 1, } ),
                    C.Value( new Simple { Number = 6, } ),
                }),
                C.Value(new Simple { Number = 2, }),
            }.AsLimited(2));

            C.ToForest(obj).Take(2).ToArray();

            Assert.Throws<InvalidOperationException>(() => {
                C.ToForest(obj).Take(3).ToArray();
            });
        }

        [Fact]
        public void ToFlatTest(){
            var obj = C.Composite (new [] {
                C.Value(new Simple { Number = 1, }),
                C.Composite(new [] {
                    C.Value( new Simple { Number = 2, } ),
                    C.Value( new Simple { Number = 3, } ),
                    C.Composite(new [] {
                        C.Value( new Simple { Number = 4, } ),
                        C.Value( new Simple { Number = 5, } ),
                    }.AsLimited(1)),
                }),
                C.Value(new Simple { Number = 6, }),
            }.AsLimited(2));

            var result = C.ToFlat(obj).Take(4).ToArray();
            for(int i=1; i<=4; i++)
            {
                Assert.Equal(i, result[i-1].Number);
            }

            Assert.Throws<InvalidOperationException>(() => {
                C.ToFlat(obj).Take(5).ToArray();
            });
        }

        [Fact]
        public void CataTest(){
            var inputSeq = new[] {
                new Simple { Number = 1 },
                new Simple { Number = 2 },
                new Simple { Number = 3 },
                new Simple { Number = 4 },
                new Simple { Number = 5 },
                new Simple { Number = 6 },
            }.AsLimited(5);
            
            var pickTransformPairs = new[] {
                new C.PickTransformPair<Simple>(new Func<Simple, string>[]{
                    (x) => x.Number == 5
                            ? "1"
                            : null,
                }, (x) => {
                        return x.ToArray()[0] == "1"
                            ? new[]{ "2", "3", "4",}
                            : new string[]{};
                    }),
            };

            var result = C.Cata(pickTransformPairs, inputSeq).ToArray();
        }

        [Fact]
        public void CataTestLazy(){
            var inputSeq = new[] {
                new Simple { Number = 1 },
                new Simple { Number = 2 },
                new Simple { Number = 3 },
                new Simple { Number = 4 },
                new Simple { Number = 5 },
                new Simple { Number = 6 },
            }.AsLimited(5);
            
            var pickTransformPairs = new[] {
                new C.PickTransformPair<Simple>(new Func<Simple, string>[]{
                    (x) => x.Number == 6
                            ? "1"
                            : null,
                }, (x) => {
                        return x.ToArray()[0] == "1"
                            ? new[]{ "2", "3", "4",}
                            : new string[]{};
                    }),
            };
            
            Assert.Throws<InvalidOperationException>(() => {
                C.Cata(pickTransformPairs, inputSeq).ToArray();
            });
        }

        [Fact]
        public void AnaTest () {
            var scn = new Func<Simple, Simple[]>[] {
                    x => x.Number == 1
                        ? new [] { new Simple { Number = 2, }, new Simple { Number = 3, }}
                        : new[] { x },
                    x => x.Number == 2
                        ? new [] { new Simple { Number = 4, }, new Simple { Number = 5, }}
                        : new[] { x },
                };

            var obj = C.Composite (new [] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
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