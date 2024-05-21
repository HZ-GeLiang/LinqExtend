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
            Assert.AreEqual(new ArrayList().AnyEnhance(), false);
            Assert.AreEqual(new ArrayList() { 1 }.AnyEnhance(), true);

            Assert.AreEqual(new StringBuilder().AnyEnhance(), false);
            Assert.AreEqual(new StringBuilder("1").AnyEnhance(), true);

            Assert.AreEqual("".AnyEnhance(), false);
            Assert.AreEqual("1".AnyEnhance(), true);
        }

    }
}