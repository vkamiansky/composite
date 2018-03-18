using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests.Composites
{
    using SimpleComposite = Composite<Simple>.Composite;
    using SimpleValue = Composite<Simple>.Value;

    public class SelectTests
    {
        [Fact]
        public void TransformationTest()
        {
            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
                C.Composite(new[] {
                    C.Value( new Simple { Number = 1, } ),
                    C.Value( new Simple { Number = 7, } ),
                }),
                C.Value( new Simple { Number = 8, } ),
            });

            var result = obj.Select(x => x.Number == 1
                                         ? 4
                                         : x.Number == 6
                                           ? 5
                                           : x.Number);

            Assert.Equal("[ 4, 5, [ 4, 7 ], 8 ]", result.AsString(x => x.ToString()));
        }
    }
}