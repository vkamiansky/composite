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

            Assert.Throws<Exception>(() => {
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

            Assert.Throws<Exception>(() => {
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
            
            Assert.Throws<Exception>(() => {
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