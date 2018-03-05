using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests {

    public class AsBatchedTests {

        [Fact]
        public void MultiItemBatchesFormationTest () {
            var inputSequence = Enumerable.Range(1, 10)
                .AsLimited(6);
            var batches = inputSequence.AsBatched(9, x => x)
                .Take(2)
                .ToArray();

            Assert.Equal(new []{1, 2, 3}, batches[0]);
            Assert.Equal(new []{4, 5}, batches[1]);
        }

        [Fact]
        public void SingleItemBatchesFormationTest () {
            var inputSequence = Enumerable.Range(6, 10)
                .AsLimited(4);
            var batches = inputSequence.AsBatched(7, x => x)
                .Take(3)
                .ToArray();

            Assert.Equal(new []{6}, batches[0]);
            Assert.Equal(new []{7}, batches[1]);
            Assert.Equal(new []{8}, batches[2]);
        }

        [Fact]
        public void ComplexBatchesFormationTest () {
            var inputSequence = new[]{8, 1, 2, 4, 8, 1}
                .AsLimited(6);
            var batches = inputSequence.AsBatched(7, x => x)
                .Take(4)
                .ToArray();

            Assert.Equal(new []{8}, batches[0]);
            Assert.Equal(new []{1, 2, 4}, batches[1]);
            Assert.Equal(new []{8}, batches[2]);
            Assert.Equal(new []{1}, batches[3]);
        }

        [Fact]
        public void InvalidParametersTest()
        {
            var inputSequence = Enumerable.Range(1, 10).AsLimited(0);

            Assert.Throws<ArgumentException>(() => inputSequence.AsBatched(0, x => x).ToArray());
            Assert.Throws<ArgumentException>(() => inputSequence.AsBatched(-1, x => x).ToArray());

            IEnumerable<int> badSequence = null;
            Assert.Throws<ArgumentNullException>(() => badSequence.AsBatched(1, x => x).ToArray());
        }

        [Fact]
        public void GetElementSizeRedundantCallsTest () {
            var callsCount = 0;
            var inputSequence = Enumerable.Range(1, 10)
                .AsLimited(6);
            var batches = inputSequence.AsBatched(10, x =>
                {
                    callsCount++;
                    return x;
                })
                .Take(2)
                .ToArray();

            Assert.Equal(6, callsCount);
        }

        [Fact]
        public void SourceRedundantWalkthroughTest () {
            var callsCount = 0;
            var inputSequence = Enumerable.Range(1, 10)
                .AsLimited(7)
                .WithSideEffect(_=> callsCount++);
            var batches = inputSequence.AsBatched(7, x => x)
                .Take(4)
                .ToArray();

            Assert.Equal(7, callsCount);
        }
    }
 }