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


        // 测试值类型键（如 int）的筛选和连接功能

        [TestMethod]
        public void AggregateToString_WithValueTypeKeys_FiltersAndConcatenates()
        {
            // Arrange
            var dictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
                { 4, "four" }
            };
            var keys = dictionary.Keys;
            Func<int, bool> predicate = x => x % 2 == 0; // 筛选偶数
            string separator = ", ";

            // Act
            var result = keys.AggregateToString(predicate, separator);

            // Assert
            Assert.AreEqual("2, 4", result);
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

            {
                var peopleList = new List<People>()
                {
                    new People {Id = 1},
                    new People {Id = 2},
                    new People {Id = 3},
                };

                var str1 = peopleList.AggregateToString(a => $@"{a.Id}", "\r\n");
                Assert.AreEqual(str1, "1\r\n2\r\n3");
            }
        }
    }
}