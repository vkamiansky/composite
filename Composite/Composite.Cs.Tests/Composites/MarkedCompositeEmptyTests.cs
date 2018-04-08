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
        public void LazyOutputTest(){
            var result = MarkedComposite.Empty<string, Simple>("mark").AsEnumerable().AllowTake(0);

            Assert.Throws<InvalidOperationException>(() =>
            {
                result.ToArray();
            });
        }
    }
}