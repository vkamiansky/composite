using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests {

    public class AsPagedTests {

        [Fact]
        public void PagesFormationTest () {
            var inputSequence = Enumerable.Range(1, 10).AsLimited(6);
            var pages = inputSequence.AsPaged(2).Take(3).ToArray();

            Assert.Equal(new []{1,2}, pages[0]);
            Assert.Equal(new []{3,4}, pages[1]);
            Assert.Equal(new []{5,6}, pages[2]);
        }
    }
 }