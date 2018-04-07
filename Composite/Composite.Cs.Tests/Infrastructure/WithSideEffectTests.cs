using System;
using System.Linq;

using Xunit;

namespace Composite.Cs.Tests.Infrastructure
{
    public class WithSideEffectTests
    {
        [Fact]
        public void SideEffectTest()
        {
            var callsCount = 0;
            var testEnumerable = Enumerable.Range(1, 5).WithSideEffect(_ => callsCount++);

            var result = testEnumerable.Take(4).ToArray();
            Assert.Equal(4, callsCount);
            Assert.Equal(new[]{1, 2, 3, 4}, result);
        }

        [Fact]
        public void TrivialParametersTest()
        {
            var callsCount = 0;

            var result = Enumerable.Empty<int>().WithSideEffect(_ => callsCount++).ToArray();
            Assert.Equal(0, callsCount);
            Assert.Equal(new int[]{}, result);
        }
    }
}