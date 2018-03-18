using System;
using System.Linq;

using Xunit;

namespace Composite.Cs.Tests.Enumerables
{
    public class AsEnumerableTests
    {
        [Fact]
        public void AsEnumerableTest()
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

            var result = obj.AsEnumerable().Take(4).ToArray();
            for (int i = 1; i <= 4; i++)
            {
                Assert.Equal(i, result[i - 1].Number);
            }

            Assert.Throws<InvalidOperationException>(() =>
            {
                obj.AsEnumerable().Take(5).ToArray();
            });
        }
    }
}