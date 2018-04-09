using System;
using System.Linq;

using Xunit;

using Composite;
using Composite.Cs.Tests.Infrastructure;

namespace Composite.Cs.Tests.Composites
{
    public class MarkedCompositeEmptyTests
    {
        [Fact]
        public void GenerationTest(){
            var result = MarkedComposite.Empty<string, Simple>("mark");

            Assert.Equal("( mark )[ ]", result.ToStringShort());
        }
    }
}