using LinqExtend.EF.Test.EF;
using LinqExtend.Test;
using Microsoft.EntityFrameworkCore;

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

            List<BookDto>? queryList;

            {
                // 手动编写映射
                var query = ctx.Books.Select(a => new BookDto()
                {
                    Id = a.Id,
                    PubTime = a.PubTime,
                    Price = a.Price,
                    Publisher = a.Publisher
                });

                var sql = query.ToQueryString();
                Assert.AreEqual(sql, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");
                queryList = query.ToList();
            }

            {

                //使用 SelectMap 创建表达式
                //1:分开写
                var exp = ExpressionHelper.SelectMap<Book, BookDto>();
                var selectMapQuery = ctx.Books.Select(exp);

                //1:直接写里面
                //var selectMapQuery = ctx.Books.Select(ExpressionHelper.SelectMap<Book, BookDto>());
                var sql_selectMap = selectMapQuery.ToQueryString();
                Assert.AreEqual(sql_selectMap, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");

                var selectMapList = selectMapQuery.ToList();

                CollectionAssert.AreEqual(queryList, selectMapList);

            }

            {
                //通过泛型封装, 动态的查询出想要的dto
                var props = typeof(BookDto).GetProperties().Select(p => p.Name).ToArray();

                //selectMapQuery是 BookDto 和 Book 共同的属性
                var selectMapQuery = ctx.Set<Book>().Select(ExpressionHelper.SelectMap<Book>(props));

                var sql_selectMap = selectMapQuery.ToQueryString();
                Assert.AreEqual(sql_selectMap, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");

                var selectMapList = selectMapQuery.ToList();

                //使用
                foreach (var item in selectMapList)
                {
                    var id = item[0];
                    var PubTime = item[1];
                    var Price = item[2];
                    var Publisher = item[3];
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

            var query_tmp = from b in ctx.Books
                            select new { b, key = "_key" };

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

            var queryList = query.ToList();

            var selectMapQuery = query_tmp.SelectMap(a => new BookDto
            {
                //规则
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

            var selectMapList = selectMapQuery.ToList();
            CollectionAssert.AreEqual(queryList, selectMapList);

        }



        [TestMethod]
        public void SelectMap_值类型_Test_Case1()
        {
            //return;//因为  LinqExtend.Standard 中的 还未完成.
            using TestDbContext ctx = new TestDbContext();

            var query_tmp = from b in ctx.Students
                            select new { b };

            query_tmp.Select(a => new
            {
                a.b.NickName.Chinese
            });

            string _mapperLog = "";

            SelectExtensions.OnSelectMapLogTo = mapperLog =>
            {
                _mapperLog = mapperLog;
            };

            var selectMapQuery = query_tmp.SelectMap(a => new StudentDto
            {
                //NickName = new MultilingualString(a.b.NickName.Chinese, a.b.NickName.English)
                //NickName = new MultilingualString()
                //{
                //    Chinese = a.b.NickName.Chinese,
                //    English = a.b.NickName.English
                //}
            })
            ;

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

            /*

             SELECT [t].[Id], [t].[IsDel], [t].[IsDel2], [t].[Price], [t].[PubTime], [t].[Publisher], [t].[Publisher2], [t].[BookInfo_AuthorName], [t].[BookInfo_Title]
            FROM [T_Books] AS [t]
            WHERE ([t].[IsDel] IS NOT NULL) AND ([t].[IsDel] = CAST(1 AS bit))*/


            using TestDbContext ctx = new TestDbContext();

            //var sql1 = ctx.Books
            // .Where(b => b.IsDel.HasValue && b.IsDel == true)
            // .Count();

            //var sql12 = ctx.Books
            //     .Where(b => b.IsDel == true)
            //     .Count();

            var sql1 = ctx.Books
              .Where(b => b.IsDel == null || b.IsDel == false)
                .Count();

            var sql2 = ctx.Books
                .Where(b => b.IsDel != true)
                .Count();


            Console.WriteLine(sql1);
        }
    }
}