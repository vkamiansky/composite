using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests.Composites
{
    using SimpleComposite = Composite<Simple>.Composite;
    using SimpleValue = Composite<Simple>.Value;

    public class ForkTests
    {
        [Fact]
        public void TransformationTest()
        {
            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
            });

            var result = obj.Fork(x => x.Number == 1, _ => new[]{new Simple { Number = 4, }, new Simple { Number = 5, }});

            Assert.Equal("[ [ 4, 5 ], 6 ]", result.AsString(x => x.Number.ToString()));
        }
    }
}