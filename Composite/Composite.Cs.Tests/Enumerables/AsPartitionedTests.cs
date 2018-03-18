using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests.Enumerables
{

    public class AsPartitionedTests
    {

        [Fact]
        public void PartitionsFormationTest()
        {
            var inputSequence = Enumerable.Range(1, 10).AsLimited(6);
            var partitions = inputSequence.AsPartitioned(3);

            var arr1 = partitions[0].Take(2).ToArray();
            var arr2 = partitions[1].Take(2).ToArray();
            var arr3 = partitions[2].Take(2).ToArray();

            Assert.Equal(new[] { 1, 2 }, arr1);
            Assert.Equal(new[] { 3, 4 }, arr2);
            Assert.Equal(new[] { 5, 6 }, arr3);
        }

        [Fact]
        public void SourceRedundantWalkthroughTest()
        {
            var callsCount = 0;
            var inputSequence = Enumerable
                .Range(1, 10)
                .WithSideEffect(_ => callsCount++);
            var partitions = inputSequence.AsPartitioned(3);

            partitions.Select(x => x.Take(2).ToArray()).ToArray();

            Assert.Equal(6, callsCount);
        }

        [Fact]
        public void InvalidParametersTest()
        {
            var inputSequence = Enumerable.Range(1, 10).AsLimited(0);

            Assert.Throws<ArgumentException>(() => inputSequence.AsPartitioned(0).ToArray());
            Assert.Throws<ArgumentException>(() => inputSequence.AsPartitioned(-1).ToArray());

            IEnumerable<int> badSequence = null;
            Assert.Throws<ArgumentNullException>(() => badSequence.AsPartitioned(1).ToArray());
        }
    }
}