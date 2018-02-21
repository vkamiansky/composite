using System;
using System.Linq;

using Xunit;

using Composite;

namespace Composite.Cs.Tests
{
    public class CataTests
    {
        [Fact]
        public void LazyInputEnumerableTest()
        {
            var inputSeq = Enumerable.Range(1, 10)
                .Select(x => new Simple { Number = x, });;

            var checkTransformRules = new[] {
                new CheckTransformRule<Simple, string>(new Func<Simple, bool>[]{
                    (x) => x.Number == 5,
                }, (x) => {
                    var num = x[0].Number;
                    return new[]{ num + "2", num + "3", num + "4",};
                }),
            };

            var result = inputSeq.AsLimited(5).Cata(checkTransformRules).ToArray();
            Assert.Equal(new[]{"52", "53", "54"}, result);

            Assert.Throws<InvalidOperationException>(() =>
            {
                inputSeq.AsLimited(4).Cata(checkTransformRules).ToArray();
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

            var result = inputSeq.Cata(checkTransformRules).Take(4).ToArray();
            Assert.Equal(new[]{"32", "43", "52", "53"}, result);

            Assert.Throws<InvalidOperationException>(() =>
            {
                inputSeq.Cata(checkTransformRules).Take(5).ToArray();
            });
        }
    }
}