using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests
{
    public class ToComponentsTests
    {
        [Fact]
        public void LazyOutputTest(){
            var obj = C.Composite (new [] {
                C.Value(new Simple { Number = 2, }),
                C.Composite(new [] {
                    C.Value(new Simple { Number = 1, }),
                    C.Value(new Simple { Number = 6, }),
                }),
                C.Value(new Simple { Number = 2, }),
            }.AsLimited(2));

            obj.ToComponents().Take(2).ToArray();

            Assert.Throws<InvalidOperationException>(() =>
            {
                obj.ToComponents().Take(3).ToArray();
            });
        }
    }
}