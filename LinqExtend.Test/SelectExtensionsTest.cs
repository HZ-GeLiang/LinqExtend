using System.Data;

namespace LinqExtend.Test
{

    [TestClass]
    public class SelectExtensionsTest
    {
        [TestMethod]
        public void Select()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Rows.Add("1");
            dt.Rows.Add("2");

            var ids = dt.Rows.Select(a => Convert.ToInt32(a[0]));

            Assert.AreEqual(1, ids.First());
            Assert.AreEqual(2, ids.Last());
        }
    }
}