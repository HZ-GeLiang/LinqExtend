using LinqExtend.EF.Test.EF;
using LinqExtend.Test;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LinqExtend.EF.Test
{
    [TestClass]
    public class ExpressionHelperTest
    {
        //[TestMethod]
        //public void MakeEqual_Test()
        //{
        //    using TestDbContext ctx = new TestDbContext();
        //    var bookDto = new BookInfo
        //    {
        //        AuthorName = "2022",
        //        Title = "2022-1-1"
        //    };
        //    var sql1 = ExpressionHelperTest_Common.GetSql_MakeEqual_Test(ctx, bookDto);

        //    var sql2 = ctx.Books
        //        .Where(ExpressionHelper.MakeEqual<Book, BookInfo>(b => b.BookInfo, bookDto))
        //        //.Where(ExpressionHelper.MakeEqual((Book b) => b.BookInfo, bookDto)) //或这个写法
        //        .ToQueryString();
        //    Assert.AreEqual(sql1, sql2);
        //}

        [TestMethod]
        public void IsEmpty_Test()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsEmpty_Test(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsEmpty((Book b) => b.Publisher))
                //.Where(ExpressionHelper.IsEmpty<Book>( b => b.Publisher))//或这个写法
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsEmpty_Test2()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsEmpty_Test2(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsEmpty((Book b) => b.Publisher2))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotEmpty_Test()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotEmpty_Test(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsNotEmpty((Book b) => b.Publisher))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotEmpty_Test2()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotEmpty_Test2(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsNotEmpty((Book b) => b.Publisher2))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsDeleted_Test1()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsDeleted_Test1(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsDeleted((Book b) => b.IsDel))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsDeleted_Test2()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsDeleted_Test2(ctx);

            //.Where(b => b.IsDel2 == true)
            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsDeleted((Book b) => b.IsDel2))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsDeleted_Test3()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsDeleted_Test3(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsDeleted((Book b) => b.IsDel3))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsDeleted_Test4()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsDeleted_Test4(ctx);

            //.Where(b => b.IsDel2 == true)
            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsDeleted((Book b) => b.IsDel4))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotDeleted_Test1()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotDeleted_Test1(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsNotDeleted((Book b) => b.IsDel))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotDeleted_Test2()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotDeleted_Test2(ctx);

            //.Where(b => b.IsDel2 == true)
            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsNotDeleted((Book b) => b.IsDel2))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotDeleted_Test3()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotDeleted_Test3(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsNotDeleted((Book b) => b.IsDel3))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotDeleted_Test4()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotDeleted_Test4(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsNotDeleted((Book b) => b.IsDel4))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void SelectMap_DbSet_Test()
        {
            using TestDbContext ctx = new TestDbContext();

            {
                IQueryable<BookDto>? query;
                {
                    // 手动编写映射(利用语法糖来创建一个 Expression<Func<Book, BookDto>> 对象)
                    query = ctx.Books.Select(a => new BookDto()
                    {
                        Id = a.Id,
                        PubTime = a.PubTime,
                        Price = a.Price,
                        Publisher = a.Publisher
                    });

                    var sql = query.ToQueryString();
                    Assert.AreEqual(sql, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");

                }


                IQueryable<BookDto> selectMapQuery;
                {
                    //使用 SelectMap 创建表达式
                    {

                        //1.分开写
                        Expression<Func<Book, BookDto>>? exp = ExpressionHelper.SelectMap<Book, BookDto>();
                        selectMapQuery = ctx.Books.Select(exp);
                        var sql_selectMap = selectMapQuery.ToQueryString();
                        Assert.AreEqual(sql_selectMap, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");
                    }
                    {
                        //2.直接写里面
                        selectMapQuery = ctx.Books.Select(ExpressionHelper.SelectMap<Book, BookDto>());
                        var sql_selectMap = selectMapQuery.ToQueryString();
                        Assert.AreEqual(sql_selectMap, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");
                    }

                }


                {
                    //List<BookDto>? queryList = query.ToList();

                    //List<BookDto>? selectMapList = selectMapQuery.ToList();

                    //CollectionAssert.AreEqual(queryList, selectMapList);
                }
            }


            {
                //通过泛型封装, 获得想要查询的属性名称
                var props = typeof(BookDto).GetProperties().Select(p => p.Name).ToArray();

                //selectMapQuery是 BookDto 和 Book 共同的属性
                var selectMapQuery = ctx.Set<Book>().Select(ExpressionHelper.SelectMap<Book>(props));

                var sql_selectMap = selectMapQuery.ToQueryString();
                Assert.AreEqual(sql_selectMap, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");

                {
                    //var selectMapList = selectMapQuery.ToList();

                    ////使用
                    //foreach (var item in selectMapList)
                    //{
                    //    var id = item[0];
                    //    var PubTime = item[1];
                    //    var Price = item[2];
                    //    var Publisher = item[3];
                    //}
                }
            }

            {
                //null值处理测试
                var exp = ExpressionHelper.SelectMap<Book>(null); //本质是: {a => new [] {}}
                var expStr = exp.ToString();
                Assert.AreEqual(expStr, $@"Param_0 => new [] {{}}");
            }
        }

        [TestMethod]
        public void SelectMap_object2Linq_Test()
        {
            //return;//因为  LinqExtend.Standard 中的 还未完成.
            using TestDbContext ctx = new TestDbContext();

            var query_tmp = from b in ctx.Books select new { b, key = "_key" };

            var query = query_tmp.Select(a => new BookDto()
            {
                Key = "_key",
                Id = a.b.Id,
                PubTime = a.b.PubTime,
                Price = a.b.Price,
                Publisher = a.b.Publisher,
            });

            var sql_query = query.ToQueryString();
            Assert.AreEqual(sql_query, $@"SELECT N'_key' AS [Key], [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");

            var selectMapQuery = query_tmp.SelectMap(a => new BookDto
            {
                //具体的规则
                Key = a.key
            });

            /* 详细的 SelectMap 如下
Key = a.key
Id = a.b.Id
PubTime = a.b.PubTime
Price = a.b.Price
Publisher = a.b.Publisher
            */

            var sql_selectMap = selectMapQuery.ToQueryString();

            Assert.AreEqual(sql_selectMap, $@"SELECT N'_key' AS [Key], [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");

            //var queryList = query.ToList();
            //var selectMapList = selectMapQuery.ToList();
            //CollectionAssert.AreEqual(queryList, selectMapList);
        }

        [TestMethod]
        public void SelectMap_值类型_Test_Case1()
        {
            //return;//因为  LinqExtend.Standard 中的 还未完成.
            using TestDbContext ctx = new TestDbContext();

            var query_tmp = from b in ctx.Students select new { b };

            query_tmp.Select(a => new
            {
                a.b.NickName.Chinese
            });

            string _mapperLog = "";

            SelectExtensions.OnSelectMapLogTo = mapperLog =>
            {
                _mapperLog = mapperLog;
            };

            //m,IQuerable/自动Select,示例
            var selectMapQuery = query_tmp.SelectMap(a => new StudentDto
            {
                //NickName = new MultilingualString(a.b.NickName.Chinese, a.b.NickName.English)
                //NickName = new MultilingualString()
                //{
                //    Chinese = a.b.NickName.Chinese,
                //    English = a.b.NickName.English
                //}
            });

            var sql_selectMap = selectMapQuery.ToQueryString();

            Assert.AreEqual(_mapperLog, $@"Id = a.b.Id
UserName = a.b.UserName
Gender = a.b.Gender
NickName = new MultilingualStringDto(a.b.NickName.Chinese, null) {{English = a.b.NickName.English}}
");
        }


        [TestMethod]
        public void SelectMap_值类型_Test_Case1_多一个参数()
        {
            //return;//因为  LinqExtend.Standard 中的 还未完成.
            using TestDbContext ctx = new TestDbContext();

            var query_tmp = from b in ctx.Students select new { b };

            query_tmp.Select(a => new
            {
                a.b.NickName.Chinese
            });

            string _mapperLog = "";

            SelectExtensions.OnSelectMapLogTo = mapperLog =>
            {
                _mapperLog = mapperLog;
            };

            var selectMapQuery = query_tmp.SelectMap(a => new StudentDto_多一个参数
            {

            });

            var sql_selectMap = selectMapQuery.ToQueryString();

            Assert.AreEqual(_mapperLog, $@"Id = a.b.Id
UserName = a.b.UserName
Gender = a.b.Gender
NickName = new MultilingualStringDto(a.b.NickName.Chinese, null) {{English = a.b.NickName.English}}
");
        }

        [TestMethod]
        public void Main()
        {
            using TestDbContext ctx = new TestDbContext();



            var query1 = ctx.Books.Where(b => b.IsDel == null || b.IsDel == false);
            var query2 = ctx.Books.Where(b => b.IsDel != true);

            //var count1 = query1.Count();
            //var count2 = query2.Count();

            var sql1 = query1.ToQueryString();
            var sql2 = query2.ToQueryString();

            Assert.IsTrue(sql1.Contains("WHERE ([t].[IsDel] = CAST(0 AS bit)) OR ([t].[IsDel] IS NULL)"));
            Assert.IsTrue(sql2.Contains("WHERE ([t].[IsDel] <> CAST(1 AS bit)) OR ([t].[IsDel] IS NULL)"));
        }
    }
}