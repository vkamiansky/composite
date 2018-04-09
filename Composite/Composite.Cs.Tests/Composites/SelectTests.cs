using System;
using System.Linq;

using Composite.Cs.Tests.Infrastructure;

using Xunit;

namespace Composite.Cs.Tests.Composites
{
    public class SelectTests
    {
        [Fact]
        public void TransformationTest()
        {
            var obj = Composite.Create(new[] {
                Composite.CreateValue( new Simple { Number = 1, } ),
                Composite.CreateValue( new Simple { Number = 6, } ),
                Composite.Create(new[] {
                    Composite.CreateValue( new Simple { Number = 1, } ),
                    Composite.CreateValue( new Simple { Number = 7, } ),
                }),
                Composite.CreateValue( new Simple { Number = 8, } ),
            });

            var result = obj.Select(x => x.Number == 1
                                         ? 4
                                         : x.Number == 6
                                           ? 5
                                           : x.Number);

            Assert.Equal("[ 4, 5, [ 4, 7 ], 8 ]", result.ToStringShort());
        }

        [Fact]
        public void LazyInputTest()
        {
            var obj = Composite.Create(new[] {
                Composite.CreateValue( new Simple { Number = 1, } ),
                Composite.Create(new[] {
                    Composite.CreateValue( new Simple { Number = 6, } ),
                    Composite.CreateValue( new Simple { Number = 7, } ),
                }.AllowTake(1)),
                Composite.CreateValue( new Simple { Number = 8, } ),
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
            var obj = Composite.Create(new[] { Composite.CreateValue(new Simple()), });

            Assert.Throws<ArgumentNullException>(() => obj.Select<Simple, int>(null));
        }
    }
}