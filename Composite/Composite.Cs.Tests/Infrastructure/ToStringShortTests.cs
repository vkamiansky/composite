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
            var inputComposite = C.MarkedComposite(string.Empty, new[] {
                C.MarkedComposite("inner composite", new[]{
                    C.MarkedValue("mark", new Simple { Number = 1, })
                })
            });

            Assert.Equal("[ ( inner composite )[ ( mark )1 ] ]", inputComposite.ToStringShort());
        }

        [Fact]
        public void MarkedTrivialParametersTest()
        {
            var inputComposite = C.MarkedComposite(string.Empty, new Composite<string,int>[]{});
            Assert.Equal("[ ]", inputComposite.ToStringShort());

            inputComposite = C.MarkedComposite("mark", new Composite<string,int>[]{});
            Assert.Equal("( mark )[ ]", inputComposite.ToStringShort());

            inputComposite = C.MarkedComposite(string.Empty, new[]{C.MarkedValue(string.Empty, 1)});
            Assert.Equal("[ 1 ]", inputComposite.ToStringShort());

            inputComposite = C.MarkedComposite(string.Empty, new[]{C.MarkedValue("mark", 1)});
            Assert.Equal("[ ( mark )1 ]", inputComposite.ToStringShort());

            inputComposite = C.MarkedValue(string.Empty, 1);
            Assert.Equal("1", inputComposite.ToStringShort());

            inputComposite = C.MarkedValue("mark", 1);
            Assert.Equal("( mark )1", inputComposite.ToStringShort());
        }
    }
}