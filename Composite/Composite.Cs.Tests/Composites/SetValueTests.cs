using System;
using System.Linq;

using Xunit;
using Composite.Cs.Tests.Infrastructure;

namespace Composite.Cs.Tests.Composites
{
    public class SetValueTests
    {
        [Fact]
        public void TransformationTest()
        {
            var obj = MarkedComposite.Create(string.Empty, new[] {
                MarkedComposite.Create("inner composite", new[]{
                    MarkedComposite.CreateValue("mark", new Simple { Number = 1, })
                })
            });

            var result = obj.SetValue(
                "inner composite",
                "mark",
                new Simple { Number = 2, },
                (x, y) => x == y);

            Assert.Equal("[ ( inner composite )[ ( mark )2 ] ]", result.ToStringShort());

            result = obj.SetValue(
                "inner composite",
                "mark2",
                new Simple { Number = 2, },
                (x, y) => x == y);

            Assert.Equal("[ ( inner composite )[ ( mark )1, ( mark2 )2 ] ]", result.ToStringShort());
        }

        [Fact]
        public void TrivialCaseTest()
        {
            var obj = MarkedComposite.CreateValue("mark", new Simple { Number = 1, });

            var result = obj.SetValue(
                string.Empty,
                "mark",
                new Simple { Number = 2, },
                (x, y) => x == y);

            Assert.Equal("( mark )1", result.ToStringShort());
        }
    }
}