using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests {

    public class AsBatchedTests {

        [Fact]
        public void BatchesFormationTest () {
            var inputSequence = Enumerable.Range(1, 10)
                .AsLimited(6);
            var batches = inputSequence.AsBatched(10, x => x)
                .Take(2)
                .ToArray();

            Assert.Equal(new []{1, 2, 3, 4}, batches[0]);
            Assert.Equal(new []{5}, batches[1]);
        }
    }
 }