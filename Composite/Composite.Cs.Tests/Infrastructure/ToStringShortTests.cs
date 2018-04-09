using Xunit;

namespace Composite.Cs.Tests.Infrastructure
{
    public class ToStringShortTests
    {
        [Fact]
        public void TransformationTest()
        {
            var inputComposite = Composite.Create(new[] {
                Composite.Create(new[]
                {
                    Composite.CreateValue(new Simple { Number = 1, }),
                    Composite.CreateValue(new Simple { Number = 2, }),
                    Composite.CreateValue(new Simple { Number = 3, })
                }),
                Composite.CreateValue( new Simple { Number = 6, } ),
            });

            Assert.Equal("[ [ 1, 2, 3 ], 6 ]", inputComposite.ToStringShort());
        }

        [Fact]
        public void TrivialParametersTest()
        {
            var inputComposite = Composite.Create(new Composite<int>[] {});
            Assert.Equal("[ ]", inputComposite.ToStringShort());

            inputComposite = Composite.Create(new[]{Composite.CreateValue(1)});
            Assert.Equal("[ 1 ]", inputComposite.ToStringShort());

            inputComposite = Composite.CreateValue(1);
            Assert.Equal("1", inputComposite.ToStringShort());
        }

        [Fact]
        public void MarkedTransformationTest()
        {
            var inputComposite = MarkedComposite.Create(string.Empty, new[] {
                MarkedComposite.Create("inner composite", new[]{
                    MarkedComposite.CreateValue("mark", new Simple { Number = 1, })
                })
            });

            Assert.Equal("[ ( inner composite )[ ( mark )1 ] ]", inputComposite.ToStringShort());
        }

        [Fact]
        public void MarkedTrivialParametersTest()
        {
            var inputComposite = MarkedComposite.Create(string.Empty, new Composite<string,int>[]{});
            Assert.Equal("[ ]", inputComposite.ToStringShort());

            inputComposite = MarkedComposite.Create("mark", new Composite<string,int>[]{});
            Assert.Equal("( mark )[ ]", inputComposite.ToStringShort());

            inputComposite = MarkedComposite.Create(string.Empty, new[]{MarkedComposite.CreateValue(string.Empty, 1)});
            Assert.Equal("[ 1 ]", inputComposite.ToStringShort());

            inputComposite = MarkedComposite.Create(string.Empty, new[]{MarkedComposite.CreateValue("mark", 1)});
            Assert.Equal("[ ( mark )1 ]", inputComposite.ToStringShort());

            inputComposite = MarkedComposite.CreateValue(string.Empty, 1);
            Assert.Equal("1", inputComposite.ToStringShort());

            inputComposite = MarkedComposite.CreateValue("mark", 1);
            Assert.Equal("( mark )1", inputComposite.ToStringShort());
        }
    }
}