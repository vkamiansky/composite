using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests
{
    public class AccumulateSelectManyTests
    {
        [Fact]
        public void LazyInputEnumerableTest()
        {
            var inputSeq = Enumerable.Range(1, 10)
                .Select(x => new Simple { Number = x, });

            var checkTransformRules = new[] {
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 5,
                }, (x) => {
                    var num = x[0].Number;
                    return new[]{ num + "2", num + "3", num + "4",};
                }),
            };

            var result = inputSeq.AsLimited(5).AccumulateSelectMany(checkTransformRules).ToArray();
            Assert.Equal(new[] { "52", "53", "54" }, result);

            Assert.Throws<InvalidOperationException>(() =>
            {
                inputSeq.AsLimited(4).AccumulateSelectMany(checkTransformRules).ToArray();
            });
        }

        [Fact]
        public void LazyOutputEnumerableTest()
        {
            var inputSeq = Enumerable.Range(1, 10)
                .Select(x => new Simple { Number = x, });

            var checkTransformRules = new[] {
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 5,
                }, (x) => {
                    var num = x[0].Number;
                    return new[]{ num + "2", num + "3", num + "4",}.AsLimited(2);
                }),
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 3,
                    (x) => x.Number == 4,
                }, (x) => {
                    return new[]{ x[0].Number + "2", x[1].Number + "3", };
                })
            };

            var result = inputSeq.AccumulateSelectMany(checkTransformRules).Take(4).ToArray();
            Assert.Equal(new[] { "32", "43", "52", "53" }, result);

            Assert.Throws<InvalidOperationException>(() =>
            {
                inputSeq.AccumulateSelectMany(checkTransformRules).Take(5).ToArray();
            });
        }

        [Fact]
        public void InputRedundantWalkthroughTest()
        {
            var callsCount = 0;
            var inputSeq = Enumerable.Range(1, 10)
                .WithSideEffect(_ => callsCount++)
                .Select(x => new Simple { Number = x, });

            var checkTransformRules = new[] {
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 5,
                }, (x) => new[]{ "2" }),
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 3,
                }, (x) => new[]{ "3" }),
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 3,
                }, (x) => new[]{ "4" }),
            };

            inputSeq.AccumulateSelectMany(checkTransformRules).ToArray();
            Assert.Equal(5, callsCount);
        }

        [Fact]
        public void OutputRedundantWalkthroughTest()
        {
            var callsCount = 0;
            var inputSeq = Enumerable.Range(1, 10)
                .Select(x => new Simple { Number = x, });

            var checkTransformRules = new[] {
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 5,
                }, (x) => new[]{ "2", "3", "4" }.WithSideEffect(_ => callsCount++)),
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 3,
                }, (x) => new[]{ "6", "9", "8" }.WithSideEffect(_ => callsCount++)),
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 3,
                }, (x) => new[]{ "7", "5", "1" }.WithSideEffect(_ => callsCount++)),
            };

            inputSeq.AccumulateSelectMany(checkTransformRules).ToArray();
            Assert.Equal(9, callsCount);
        }

        [Fact]
        public void InvalidParametersTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var goodRules = new[] {
                    new CheckTransformRule<int, string>(
                        new Func<int, bool>[]{(x) => true,},
                        (x) => new[] {""}),
                        };
                IEnumerable<int> badSource = null;
                badSource.AccumulateSelectMany<int, string>(goodRules);
            });

            var goodSource = Enumerable.Range(1, 10);

            Assert.Throws<ArgumentNullException>(() =>
            {
                goodSource.AccumulateSelectMany<int, string>(null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var badRules = new[] {
                    new CheckTransformRule<int, string>(
                        new Func<int, bool>[]{},(x) => new[] {""}),
                        };
                goodSource.AccumulateSelectMany<int, string>(badRules);
            });
        }

        [Fact]
        public void TrivialScenarioTest()
        {
            var noGoSource = Enumerable.Range(1, 10).AsLimited(0);
            var trivialRules = new CheckTransformRule<int, string>[] { };
            var result = noGoSource.AccumulateSelectMany<int, string>(trivialRules).ToArray();
            Assert.Empty(result);
        }
    }
}