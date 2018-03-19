using Xunit;

namespace Composite.Cs.Tests.Helpers
{
    public class ToStringShortTests
    {
        [Fact]
        public void TransformationTest()
        {
            var inputComposite = C.Composite(new[] {
                C.Composite(new[]
                {
                    C.Value(new Simple { Number = 1, }),
                    C.Value(new Simple { Number = 2, }),
                    C.Value(new Simple { Number = 3, })
                }),
                C.Value( new Simple { Number = 6, } ),
            });

            Assert.Equal("[ [ 1, 2, 3 ], 6 ]", inputComposite.ToStringShort());
        }

        [Fact]
        public void TrivialParametersTest()
        {
            var inputComposite = C.Composite(new Composite<int>[] {});
            Assert.Equal("[ ]", inputComposite.ToStringShort());

            inputComposite = C.Composite(new[]{C.Value(1)});
            Assert.Equal("[ 1 ]", inputComposite.ToStringShort());

            inputComposite = C.Value(1);
            Assert.Equal("1", inputComposite.ToStringShort());
        }
    }
}