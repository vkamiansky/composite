using System;
using System.Linq;

using Xunit;
using Composite.Cs.Tests.Infrastructure;

namespace Composite.Cs.Tests.Composites
{
    public class AsEnumerableTests
    {
        [Fact]
        public void AsEnumerableTest()
        {
            var obj = Composite.Create(new[] {
                Composite.CreateValue(new Simple { Number = 1, }),
                Composite.Create(new [] {
                    Composite.CreateValue(new Simple { Number = 2, }),
                    Composite.CreateValue(new Simple { Number = 3, }),
                    Composite.Create(new [] {
                        Composite.CreateValue(new Simple { Number = 4, }),
                        Composite.CreateValue(new Simple { Number = 5, }),
                    }.AllowTake(1)),
                }),
                Composite.CreateValue(new Simple { Number = 6, }),
            }.AllowTake(2));

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