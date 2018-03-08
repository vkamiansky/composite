using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

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
    }
}