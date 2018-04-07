using Xunit;

namespace Composite.Cs.Tests.Infrastructure
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

        [Fact]
        public void MarkedTransformationTest()
        {
            var inputComposite = C.MarkedComposite(new[] {
                C.MarkedComposite(new[]{
                    C.MarkedValue(new Simple { Number = 1, }, "mark")
                }, "inner composite")
            },"");

            Assert.Equal("[ ( inner composite )[ ( mark )1 ] ]", inputComposite.ToStringShort());
        }

        [Fact]
        public void MarkedTrivialParametersTest()
        {
            var inputComposite = C.MarkedComposite(new Composite<string,int>[]{}, "");
            Assert.Equal("[ ]", inputComposite.ToStringShort());

            inputComposite = C.MarkedComposite(new Composite<string,int>[]{}, "mark");
            Assert.Equal("( mark )[ ]", inputComposite.ToStringShort());

            inputComposite = C.MarkedComposite(new[]{C.MarkedValue(1, "")}, "");
            Assert.Equal("[ 1 ]", inputComposite.ToStringShort());

            inputComposite = C.MarkedComposite(new[]{C.MarkedValue(1, "mark")}, "");
            Assert.Equal("[ ( mark )1 ]", inputComposite.ToStringShort());

            inputComposite = C.MarkedValue(1, "");
            Assert.Equal("1", inputComposite.ToStringShort());

            inputComposite = C.MarkedValue(1, "mark");
            Assert.Equal("( mark )1", inputComposite.ToStringShort());
        }
    }
}