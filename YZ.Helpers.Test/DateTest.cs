using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using YZ;

namespace YZ.Tests {
    [TestClass]
    public class DateTest {
        [TestMethod]
        public void TestConstraint() {
            var rnd = new Random();
            for (int i = 0; i < 10000; i++) {
                var t1 = DateTime.Now.AddDays((rnd.NextDouble() - .5) * 3);
                t1 = DateTime.Now.AddMinutes(3).Constraint(t1);
                Assert.IsTrue(t1.Year==DateTime.Now.Year);
            }
        }
    }
}
