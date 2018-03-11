using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests
{
    using SimpleComposite = Composite<Simple>.Composite;
    using SimpleValue = Composite<Simple>.Value;

    public class AnaTests
    {
        [Fact]
        public void UnfoldTest()
        {
            var scn = new[] {
                new CheckUnfoldRule<Simple>(
                    x => x.Number == 1,
                    x => new [] { new Simple { Number = 2, }, new Simple { Number = 3, }}
                ),
                new CheckUnfoldRule<Simple>(
                    x => x.Number == 2,
                    x => new [] { new Simple { Number = 4, }, new Simple { Number = 5, }}
                )};

            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
            });

            var result = obj.Ana(scn);

            Assert.Equal("[ [ [ 4, 5 ], 3 ], 6 ]", result.AsString(x => x.Number.ToString()));
        }
    }
}