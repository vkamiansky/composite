using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Composite;

using SimpleComposite = Composite.DataTypes.Composite<Composite.Cs.Tests.Tests.Simple>.Composite;

namespace Composite.Cs.Tests {

    public class Tests {

        public class Simple {
            public string Name { get; set; }
        }

        [Fact]
        public void UnfoldTest () {
            const string Alice = "Alice";
            const string Bob = "Bob";

            var scn = new Func<Simple, Simple[]>[] {
                    x => {
                        if (x.Name.Equals (Alice, StringComparison.InvariantCulture)) {
                            return new [] { new Simple { Name = "First", }, new Simple { Name = "Second", }, };
                        }
                        return new[]{x};
                    },
                    x => {
                        if (x.Name.Equals ("First", StringComparison.InvariantCulture)) {
                            return new [] { new Simple { Name = "Third", }, new Simple { Name = "Fourth", }, };
                        }
                        return new[]{x};
                    },
                };

            var obj = C.Composite (new [] {
                new Simple { Name = Alice, },
                new Simple { Name = Bob, },
            });

            var result = C.Ana (scn, obj);

            Assert.True(result.IsComposite);

            if (result is SimpleComposite compositeResult) {
                var resultArray = compositeResult.Item.ToArray();

                Assert.True(resultArray.Length == 2);
                Assert.True(resultArray[0].IsComposite);
                Assert.True(resultArray[1].IsValue);
            }
            else {
                Assert.True(false);
            }
        }
    }
}