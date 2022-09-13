namespace LinqExtend.Test
{


    [TestClass]
    public class OrderByExtensionsTest
    {
        internal class pepple2
        {
            public int Height { get; set; }
            public string Name { get; set; }
            public int Weight { get; set; }
        }

        [TestMethod]
        public void Test_OrderBy()
        {
            {
                var list = new List<string> { "3", "2", "4" };
                var str = list.OrderBy((a, b) =>
                {
                    return string.Compare(a, b);
                }).AggregateToString(",");
                Assert.AreEqual("2,3,4", str);

            }

            {
                List<pepple2> pepples = new List<pepple2>()
                {
                    new pepple2{ Height=100, Name="ac", Weight =1  },
                    new pepple2{ Height=99, Name="ac", Weight =3  },
                    new pepple2{ Height=100, Name="aa", Weight =2  },
                };
                var str = pepples.OrderBy((a, b) =>
                {
                    if (a.Height == b.Height && a.Name == b.Name)
                    {
                        return 0;
                    }
                    if (a.Height == b.Height)
                    {
                        return String.Compare(a.Name, b.Name);//从小到大
                    }
                    //else
                    return a.Height > b.Height ? 1 : -1; //从小到大
                }).ToList().AggregateToString(a => a.Weight, ",");
                Assert.AreEqual("3,2,1", str);
            }
        }


        public class test_001
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        [TestMethod]
        public void Test_OrderByExpression_SingField()
        {
            // 单个字段
            List<test_001> tests = new List<test_001>()
            {
                new test_001{  Id=1, Name="0" },
                new test_001{  Id=2, Name="11"},
                new test_001{  Id=4, Name="11"},
                new test_001{  Id=3, Name="11"},
                new test_001{  Id=5, Name="0"},
                new test_001{  Id=6, Name="6"},
            };

            var orderResult =
                    (from a in tests
                     orderby a.Name
                     select new test_001
                     {
                         Id = a.Id,
                         Name = a.Name
                     }
                    )
                    .ToList()
                    .Select(a => a.Id);  //  1 5 2 4 3 6 


            var orderResult2 = tests
                .OrderByExpression(a => new { a.Name })
                .ToList()
                .Select(a => a.Id)
                .AggregateToString(a => a, ",")
                ; // 准确结果应该是   1 5 2 4 3 6 

            Assert.AreEqual(orderResult2, "1,5,2,4,3,6");
        }

        [TestMethod]
        public void Test_OrderByExpression_MultField()
        {
            // 多字段
            List<test_001> tests = new List<test_001>()
            {
                new test_001{  Id=1, Name="0" },
                new test_001{  Id=2, Name="11"},
                new test_001{  Id=4, Name="11"},
                new test_001{  Id=3, Name="11"},
                new test_001{  Id=5, Name="0"},
                new test_001{  Id=6, Name="6"},
            };

            var orderResult =
                    (from a in tests
                     orderby Convert.ToInt32(a.Name), a.Id
                     select new test_001
                     {
                         Id = a.Id,
                         Name = a.Name
                     }
                    )
                    .ToList()
                    .Select(a => a.Id);  //  1 5 6 2 3 4 


            var orderResult2 = tests
                .OrderByExpression(a => new { Name = Convert.ToInt32(a.Name), a.Id })
                .ToList()
                .Select(a => a.Id)
                .AggregateToString(a => a, ",")
                ; // 准确结果应该是   1 5 6 2 3 4 

            Assert.AreEqual(orderResult2, "1,5,6,2,3,4");
        }

        [TestMethod]
        public void Test_OrderByExpression_ternary()
        {
            //支持三元表达式

            List<test_001> tests = new List<test_001>()
            {
                new test_001{  Id=1, Name="0" },
                new test_001{  Id=2, Name="11"},
                new test_001{  Id=4, Name="11"},
                new test_001{  Id=3, Name="11"},
                new test_001{  Id=5, Name=""},
                new test_001{  Id=6, Name="6"},
            };

            var orderResult =
                      (from a in tests
                       orderby string.IsNullOrEmpty(a.Name) ? -1 : Convert.ToInt32(a.Name)
                       select new test_001
                       {
                           Id = a.Id,
                           Name = a.Name
                       }
                    )
                    .ToList()
                    .Select(a => a.Id);  //  1 5 6 2 3 4 


            var orderResult2 = tests
                .OrderByExpression(a => new { Name = string.IsNullOrEmpty(a.Name) ? -1 : Convert.ToInt32(a.Name) })
                .Select(a => a.Id)
                .AggregateToString(a => a, ","); // 准确结果应该是   5 1 6 2 4 3
            Assert.AreEqual(orderResult2, "5,1,6,2,4,3");

            var orderResult3 = tests
              .OrderByExpression(a => new { Name = string.IsNullOrEmpty(a.Name) ? -1 : Convert.ToInt32(a.Name), a.Id })
              .Select(a => a.Id)
              .AggregateToString(a => a, ","); // 准确结果应该是   5 1 6 2 3 4 

            Assert.AreEqual(orderResult3, "5,1,6,2,3,4");
        }
     

    }
}