using System;
using System.Linq;

using Xunit;

using Composite.Cs.Tests.Infrastructure;

namespace Composite.Cs.Tests.Composites
{
    public class ToComponentsTests
    {
        [Fact]
        public void LazyOutputTest(){
            var obj = Composite.Create (new [] {
                Composite.CreateValue(new Simple { Number = 2, }),
                Composite.Create(new [] {
                    Composite.CreateValue(new Simple { Number = 1, }),
                    Composite.CreateValue(new Simple { Number = 6, }),
                }),
                Composite.CreateValue(new Simple { Number = 2, }),
            }.AllowTake(2));

            obj.ToComponents().Take(2).ToArray();

            Assert.Throws<InvalidOperationException>(() =>
            {
                obj.ToComponents().Take(3).ToArray();
            });
        }
    }
}