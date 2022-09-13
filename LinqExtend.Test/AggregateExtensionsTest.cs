using System.Data;
using static LinqExtend.Test.OrderByExtensionsTest;
using System.Text;

namespace LinqExtend.Test
{

    [TestClass]
    public class AggregateExtensionsTest
    {
        internal class People
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void AggregateToString()
        {


            {
                var peopleList = new List<People>()
                    {
                        new People {Id = 1},
                        new People {Id = 2},
                        new People {Id = 3},
                    };
                var str1 = peopleList.AggregateToString(a => a.Id, ",");
                Assert.AreEqual(str1, "1,2,3");

                var list = new List<int> { 4, 5, 6 };
                var str2 = list.AggregateToString(",");
                Assert.AreEqual(str2, "4,5,6");
            }

            {
                var list = new List<int> { 4 };
                var str2 = list.AggregateToString(",");
                Assert.AreEqual(str2, "4");

            }

            {
                var peopleList = new List<People>()
                    {
                        new People {Name = "1"},
                        new People {Name = "2"},
                        new People {Name = "3"},
                    };
                var str1 = peopleList.AggregateToString(a => $@"'{a.Name}'", ",");
                Assert.AreEqual(str1, "'1','2','3'");

                var list = new List<string> { "4", "5", "6" };
                var str2 = list.AggregateToString(",");
                Assert.AreEqual(str2, "4,5,6");

                str2 = list.AggregateToString(a => $@"'{a}'", ",");
                Assert.AreEqual(str1, "'1','2','3'");

            }
        }
    }
}