using System;
using System.Linq;

using Xunit;

namespace Composite.Cs.Tests.Infrastructure
{
    public class AllowTakeTests
    {
        [Fact]
        public void TransformationTest()
        {
            var limitedEnumerable = Enumerable.Range(1, 5).AllowTake(4);

            var result = limitedEnumerable.Take(4).ToArray();
            Assert.Equal(new[]{1, 2, 3, 4}, result);

            Assert.Throws<InvalidOperationException>(() => limitedEnumerable.Take(5).ToArray());
        }

        [Fact]
        public void TrivialParametersTest()
        {
            var limitedEnumerable = Enumerable.Empty<int>().AllowTake(0);
            Assert.Throws<InvalidOperationException>(() => limitedEnumerable.ToArray());

            limitedEnumerable = Enumerable.Range(1, 5).AllowTake(0);
            Assert.Throws<InvalidOperationException>(() => limitedEnumerable.Take(1).ToArray());

            limitedEnumerable = Enumerable.Empty<int>().AllowTake(1);
            limitedEnumerable.ToArray();
        }
    }
}