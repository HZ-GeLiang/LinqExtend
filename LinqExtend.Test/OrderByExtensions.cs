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
        public void Test_OrderByFunc()
        {
            {
                var list = new List<string> { "3", "2", "4" };

                //单个字段
                var str = list.OrderByFunc((a, b) =>
                {
                    return string.Compare(a, b);
                }).AggregateToString(",");
                Assert.AreEqual(str, "2,3,4");

            }

            {
                List<pepple2> pepples = new List<pepple2>()
                {
                    new pepple2{ Height=100, Name="ac", Weight =1  },
                    new pepple2{ Height=99, Name="ac", Weight =3  },
                    new pepple2{ Height=100, Name="aa", Weight =2  },
                };
                //多个字段,自己控制返回值
                var str = pepples.OrderByFunc((a, b) =>
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
                }).AggregateToString(a => a.Weight, ",");
                Assert.AreEqual(str, "3,2,1");
            }
        }

        [TestMethod]
        public void Test_OrderByFunc_ListIsNull()
        {

            {
                List<string> list = null;
                //单个字段
                var str = list.OrderByFunc((a, b) =>
                {
                    return string.Compare(a, b);
                }).AggregateToString(",");
                Assert.AreEqual(str, "");
            }


            {
                List<pepple2> pepples = null;
                //多个字段,自己控制返回值
                var str = pepples.OrderByFunc((a, b) =>
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
                }).AggregateToString(a => a.Weight, ",");
                Assert.AreEqual(str, "");
            }
        }

        public class test_001
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class test_001_select
        {
            public int Id { get; set; }
            public int Name { get; set; }
        }

        [TestMethod]
        public void Test_OrderByExpression_SingField()
        {
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
                     select a
                    )
                    .AggregateToString(a => a.Id, ",");

            // 单个字段
            var orderResult2 = tests
                .OrderByExpression(a => new { a.Name })
                .AggregateToString(a => a.Id, ","); // 准确结果应该是   1 5 2 4 3 6 

            Assert.AreEqual(orderResult2, orderResult);
        }

        [TestMethod]
        public void Test_OrderByExpression_MultField()
        {
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
                     select a
                    )
                    .AggregateToString(a => a.Id, ",");

            // 多字段
            var orderResult2 = tests
                .OrderByExpression(a => new { Name = Convert.ToInt32(a.Name), Id = a.Id })
                .AggregateToString(a => a.Id, ","); // 准确结果应该是   1 5 6 2 3 4 

            Assert.AreEqual(orderResult2, orderResult);
        }

        [TestMethod]
        public void Test_OrderByExpression_Ternary()
        {
            List<test_001> tests = new List<test_001>()
            {
                new test_001{  Id=1, Name="0" },
                new test_001{  Id=2, Name="11"},
                new test_001{  Id=4, Name="11"},
                new test_001{  Id=3, Name="11"},
                new test_001{  Id=5, Name=""},//-1
                new test_001{  Id=6, Name="6"},
            };

            var orderResult =
                      (from a in tests
                       orderby string.IsNullOrEmpty(a.Name) ? -1 : Convert.ToInt32(a.Name)
                       select a
                    )
                    .AggregateToString(a => a.Id, ",");

            //支持三元表达式
            var orderResult2 = tests
                .OrderByExpression(a => new { Name = string.IsNullOrEmpty(a.Name) ? -1 : Convert.ToInt32(a.Name) })
                .AggregateToString(a => a.Id, ","); // 准确结果应该是   5 1 6 2 4 3
            Assert.AreEqual(orderResult2, orderResult);

            var orderResult3 = tests
              .OrderByExpression(a => new { Name = string.IsNullOrEmpty(a.Name) ? -1 : Convert.ToInt32(a.Name), a.Id })
              .AggregateToString(a => a.Id, ","); // 准确结果应该是   5 1 6 2 3 4 

            Assert.AreEqual(orderResult3, "5,1,6,2,3,4");
        }


        [TestMethod]
        public void Test_OrderByExpression_ListTIsDynamic()
        {
            List<test_001> tests = new List<test_001>()
            {
                new test_001{Id=1, Name="0" },
                new test_001{Id=2, Name="11"},
                new test_001{Id=4, Name="11"},
                new test_001{Id=3, Name="11"},
                new test_001{Id=5, Name="0"},
                new test_001{Id=6, Name="6"},
            };

            {
                var orderResult =
                  (from a in
                       tests.Select(a => new
                       {
                           a.Id,
                           a.Name
                       })
                   orderby a.Name
                   select a
                  )
                  .AggregateToString(a => a.Id, ",");

                // List<匿名类> 进行排序
                var orderResult2 = tests
               .Select(a => new
               {
                   a.Id,
                   a.Name
               })
               .OrderByExpression(a => new { a.Name })
               .AggregateToString(a => a.Id, ","); // 准确结果应该是   1 5 2 4 3 6 

                Assert.AreEqual(orderResult2, orderResult);
            }

            {
                var orderResult =
                 (from a in tests
                  orderby a.Name
                  select a
                 )
                 .AggregateToString(a => a.Id, ",");

                // List<匿名类> 进行排序
                var orderResult2 = tests
               .Select(a => new
               {
                   a.Id,
                   a.Name
               })
               .OrderByExpression(a => new { a.Id })
               .AggregateToString(a => a.Id, ","); // 准确结果应该是   1,2,3,4,5,6

                Assert.AreEqual(orderResult2, "1,2,3,4,5,6");
            }
        }

        [TestMethod]
        public void Test_OrderByExpression_SelectObject()
        {
            List<test_001> tests = new List<test_001>()
            {
                new test_001{Id=1, Name="0" },
                new test_001{Id=2, Name="11"},
                new test_001{Id=4, Name="11"},
                new test_001{Id=3, Name="11"},
                new test_001{Id=5, Name="0"},
                new test_001{Id=6, Name="6"},
            };

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                //System.InvalidOperationException:“Failed to compare two elements in the array.”
                //ArgumentException: At least one object must implement IComparable.

                var orderResult =
                        (from a in tests
                         orderby Convert.ToInt32(a.Name), a.Id
                         // 不能这样写, 异常信息如上注释
                         orderby new test_001_select { Name = Convert.ToInt32(a.Name), Id = a.Id }
                         select a
                        )
                        .AggregateToString(a => a.Id, ",");
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // 一个确切的对象(提示异常,不支持这种写法, 因为 linq 2 object 也不支持
                var orderResult2 = tests
                    .OrderByExpression(a => new test_001_select
                    {
                        Id = a.Id,
                        Name = Convert.ToInt32(a.Name),
                    })
                    .AggregateToString(a => a.Id, ","); // 准确结果应该是   1 5 6 2 3 4 
            });

        }


        [TestMethod]
        public void Test_OrderByExpression_Desc()
        {
            List<test_001> tests = new List<test_001>()
            {
                new test_001{Id=1, Name="0" },
                new test_001{Id=2, Name="11"},
                new test_001{Id=4, Name="11"},
                new test_001{Id=3, Name="11"},
                new test_001{Id=5, Name="0"},
                new test_001{Id=6, Name="6"},
            };

            var orderResult2 = tests
                    .OrderByExpression(a => new { a.Name }, a => a.Id) // 不在排序字段中
                    .AggregateToString(a => a.Id, ","); //1,5,2,4,3,6
            Assert.AreEqual(orderResult2, "1,5,2,4,3,6");

            var orderResult4 = tests
                  .OrderByExpression(a => new { a.Name }, a => new { }) // 没有倒序排序字段
                  .AggregateToString(a => a.Id, ","); //1,5,2,4,3,6
            Assert.AreEqual(orderResult4, "1,5,2,4,3,6");


            var orderResult1 = tests
                   .OrderByExpression(a => new { a.Name }, a => a.Name) // 在排序字段中
                   .AggregateToString(a => a.Id, ","); //6,2,4,3,1,5
            Assert.AreEqual(orderResult1, "6,2,4,3,1,5");

            var orderResult3 = tests
                   .OrderByExpression(a => new { a.Name }, a => new { a.Id, a.Name }) // 部分字段在排序中
                   .AggregateToString(a => a.Id, ","); //6,2,4,3,1,5
            Assert.AreEqual(orderResult3, "6,2,4,3,1,5");
        }
    }
}