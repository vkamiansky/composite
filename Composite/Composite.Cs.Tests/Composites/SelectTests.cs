using System;
using System.Linq;

using Composite.Cs.Tests.Enumerables;

using Xunit;

namespace Composite.Cs.Tests.Composites
{
    public class SelectTests
    {
        [Fact]
        public void TransformationTest()
        {
            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Value( new Simple { Number = 6, } ),
                C.Composite(new[] {
                    C.Value( new Simple { Number = 1, } ),
                    C.Value( new Simple { Number = 7, } ),
                }),
                C.Value( new Simple { Number = 8, } ),
            });

            var result = obj.Select(x => x.Number == 1
                                         ? 4
                                         : x.Number == 6
                                           ? 5
                                           : x.Number);

            Assert.Equal("[ 4, 5, [ 4, 7 ], 8 ]", result.AsString(x => x.ToString()));
        }

        [Fact]
        public void LazyInputTest()
        {
            var obj = C.Composite(new[] {
                C.Value( new Simple { Number = 1, } ),
                C.Composite(new[] {
                    C.Value( new Simple { Number = 6, } ),
                    C.Value( new Simple { Number = 7, } ),
                }.AsLimited(1)),
                C.Value( new Simple { Number = 8, } ),
            });

            var baseQuery = obj.Select(x => x.Number == 1 ? 4 : x.Number == 6 ? 5 : x.Number);

            var result = baseQuery.AsEnumerable()
                                  .Take(2)
                                  .ToArray();

            Assert.Equal(4, result[0]);
            Assert.Equal(5, result[1]);

            Assert.Throws<InvalidOperationException>(() => baseQuery.AsEnumerable().Take(3).ToArray());
        }

        [Fact]
        public void InvalidParametersTest()
        {
            var obj = C.Composite(new[] { C.Value(new Simple()), });

            Assert.Throws<ArgumentNullException>(() => obj.Select<Simple, int>(null));
        }
    }
}