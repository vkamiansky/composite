using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;
using Composite.Cs.Tests.Infrastructure;

namespace Composite.Cs.Tests.Enumerables
{

    public class AsPagedTests
    {

        [Fact]
        public void PagesFormationTest()
        {
            var inputSequence = Enumerable.Range(1, 10).AllowTake(6);
            var pages = inputSequence.AsPaged(2).Take(3).ToArray();

            Assert.Equal(new[] { 1, 2 }, pages[0]);
            Assert.Equal(new[] { 3, 4 }, pages[1]);
            Assert.Equal(new[] { 5, 6 }, pages[2]);
        }

        [Fact]
        public void InvalidParametersTest()
        {
            var inputSequence = Enumerable.Range(1, 10).AllowTake(0);

            Assert.Throws<ArgumentException>(() => inputSequence.AsPaged(0).ToArray());
            Assert.Throws<ArgumentException>(() => inputSequence.AsPaged(-1).ToArray());

            IEnumerable<int> badSequence = null;
            Assert.Throws<ArgumentNullException>(() => badSequence.AsPaged(1).ToArray());
        }
    }
}