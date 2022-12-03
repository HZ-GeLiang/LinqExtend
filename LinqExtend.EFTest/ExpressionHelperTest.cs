using LinqExtend.EF;
using LinqExtend.EF.Test.EF;
using Microsoft.EntityFrameworkCore;

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