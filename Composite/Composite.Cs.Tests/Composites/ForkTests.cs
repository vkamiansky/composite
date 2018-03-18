using System;
using System.Linq;

using Composite.Cs.Tests.Enumerables;

using Xunit;

namespace Composite.Cs.Tests.Composites
{
    public class ForkTests
    {
        [Fact]
        public void TransformationTest()
        {
            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
            });

            var result = obj.Fork(x => x.Number == 1, _ => new[] { new Simple { Number = 4, }, new Simple { Number = 5, } });

            Assert.Equal("[ [ 4, 5 ], 6 ]", result.AsString(x => x.Number.ToString()));
        }

        [Fact]
        public void LazyInputTest()
        {
            var obj = C.Composite(new[] {
                C.Composite(new[]
                {
                    C.Value(new Simple { Number = 1, }),
                    C.Value(new Simple { Number = 2, }),
                    C.Value(new Simple { Number = 3, })
                }.AsLimited(2)),
                C.Value( new Simple { Number = 6, } ),
            });

            var baseQuery = obj.Fork(
                x => x.Number == 1,
                _ => new[]
                {
                    new Simple { Number = 4, },
                    new Simple { Number = 5, },
                });

            var result = baseQuery.AsEnumerable()
                .Take(3)
                .ToArray();

            Assert.Equal(4, result[0].Number);
            Assert.Equal(5, result[1].Number);

            Assert.Throws<InvalidOperationException>(() => baseQuery.AsEnumerable().Take(4).ToArray());
        }

        [Fact]
        public void LazyOutputTest()
        {
            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
            });

            var forked = obj.Fork(
                x => x.Number == 1,
                _ => new[] { new Simple { Number = 4, }, new Simple { Number = 5, }, new Simple { Number = 6, } }.AsLimited(2));

            var result = forked.AsEnumerable()
                               .Take(2)
                               .ToArray();

            Assert.Equal(4, result[0].Number);
            Assert.Equal(5, result[1].Number);

            Assert.Throws<InvalidOperationException>(() => forked.AsEnumerable().Take(3).ToArray());
        }

        [Fact]
        public void PredicateCallTest()
        {
            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
            });

            var forked = obj.Fork(
                x => x.Number == 1 ? true : throw new InvalidOperationException(),
                _ => new[] { new Simple(), new Simple(), });

            forked.AsEnumerable().Take(2).ToArray();

            Assert.Throws<InvalidOperationException>(() => forked.AsEnumerable().Take(3).ToArray());
        }

        [Fact]
        public void InvalidParametersTest()
        {
            var obj = C.Composite(new[] {
                C.Value(new Simple()),
                C.Value(new Simple()),
            });

            Assert.Throws<ArgumentNullException>(() => obj.Fork(null, null));
            Assert.Throws<ArgumentNullException>(() => obj.Fork(null, (_) => Enumerable.Empty<Simple>()));
        }
    }
}