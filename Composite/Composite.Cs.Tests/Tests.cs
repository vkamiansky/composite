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
        public void ToForestTest(){
            var obj = C.Composite (new [] {
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