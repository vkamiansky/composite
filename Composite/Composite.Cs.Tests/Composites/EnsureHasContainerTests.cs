using System;
using System.Linq;

using Xunit;
using Composite.Cs.Tests.Infrastructure;

namespace Composite.Cs.Tests.Composites
{
    public class EnsureHasContainerTests
    {
        [Fact]
        public void TransformationTest()
        {
            var obj = MarkedComposite.Create(string.Empty, new[] {
                MarkedComposite.Create("inner composite", new[]{
                    MarkedComposite.CreateValue("mark", new Simple { Number = 1, }),
                    MarkedComposite.Create("inner composite level2", new[]{
                        MarkedComposite.CreateValue("mark2", new Simple { Number = 2, })
                    })
                })
            });

            var result = obj.EnsureHasContainer(
                "inner composite",
                "inner composite level2",
                (x, y) => x == y);

            Assert.Equal("[ ( inner composite )[ ( mark )1, ( inner composite level2 )[ ( mark2 )2 ] ] ]", result.ToStringShort());

            result = obj.EnsureHasContainer(
                string.Empty,
                "inner composite2",
                (x, y) => x == y);

            Assert.Equal("[ ( inner composite )[ ( mark )1, ( inner composite level2 )[ ( mark2 )2 ] ], ( inner composite2 )[ ] ]", result.ToStringShort());
        }

        [Fact]
        public void TrivialCaseTest()
        {
            var obj = MarkedComposite.CreateValue("mark", new Simple { Number = 1, });

            var result = obj.EnsureHasContainer(
                string.Empty,
                "mark",
                (x, y) => x == y);

            Assert.Equal("( mark )1", result.ToStringShort());
        }
    }
}