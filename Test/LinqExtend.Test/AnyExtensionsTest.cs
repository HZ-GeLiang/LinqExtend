using System.Collections;
using System.Text;

namespace LinqExtend.Test
{


    [TestClass]
    public class AnyExtensionsTest
    {
        [TestMethod]
        public void Any()
        {
            Assert.AreEqual(new ArrayList().Any(), false);
            Assert.AreEqual(new ArrayList() { 1 }.Any(), true);

            Assert.AreEqual(new StringBuilder().Any(), false);
            Assert.AreEqual(new StringBuilder("1").Any(), true);

            Assert.AreEqual("".Any(), false);
            Assert.AreEqual("1".Any(), true);

        }

    }
}