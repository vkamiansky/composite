using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

using SimpleComposite = Composite.DataTypes.Composite<Composite.Cs.Tests.Tests.Simple>.Composite;

namespace Composite.Cs.Tests {

    public static class EnumerableExtensions{
        public static IEnumerable<T> AsLimitedEnumerable<T>(this T[] source, int limit)
        {
            int i = 0;
            while(true){

                if(i<limit) {
                    yield return source[i];
                    i++;
                } else {
                    throw new Exception("You've attempted to walk through an infinite sequence.");
                }
            }
        }
    }

    public class Tests {

        public class Simple {
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
            }.AsLimitedEnumerable(2));

            C.ToForest(obj).Take(2).ToArray();

            Assert.Throws<Exception>(() => {
                C.ToForest(obj).Take(3).ToArray();
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