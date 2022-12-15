using LinqExtend.EF;
using LinqExtend.EF.Test.EF;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;

namespace LinqExtend.Test
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
            var queryList = query.ToList();

            //var exp = ExpressionHelper.SelectMap<Book, BookDto>();
            //var selectMapList = ctx.Books.Select(exp).ToList();

            var selectMapQuery = ctx.Books
                .Select(ExpressionHelper.SelectMap<Book, BookDto>())
                ;
            var sql_selectMap = selectMapQuery.ToQueryString();
            Assert.AreEqual(sql_selectMap, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");

            var selectMapList = selectMapQuery.ToList();

            CollectionAssert.AreEqual(queryList, selectMapList);
        }



        [TestMethod]
        public void SelectMap_object2Linq_Test()
        {

            return;//因为  LinqExtend.Standard 中的 还未完成. 
            using TestDbContext ctx = new TestDbContext();

            var query_tmp = from b in ctx.Books
                            select new { b = b, key = "_key" };


            var query = query_tmp.Select(a => new BookDto()
            {
                Id = a.b.Id,
                PubTime = a.b.PubTime,
                Price = a.b.Price,
                Publisher = a.b.Publisher
            });

            var sql = query.ToQueryString();
            Assert.AreEqual(sql, $@"SELECT [t].[Id], [t].[PubTime], [t].[Price], [t].[Publisher]
FROM [T_Books] AS [t]");

            var queryList = query.ToList();


            var selectMapQuery = query_tmp.SelectMap(a => new BookDto
            {
                //规则            

                Key = a.key

            });
            var sql_selectMap = selectMapQuery.ToQueryString();

            var selectMapList = selectMapQuery.ToList();


            CollectionAssert.AreEqual(queryList, selectMapList);

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