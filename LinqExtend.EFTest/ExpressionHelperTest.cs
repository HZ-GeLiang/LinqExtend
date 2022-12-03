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
        public void IsNullOrEmpty_Test()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNullOrEmpty_Test(ctx);

            var sql2 = ctx.Books
                .Where(ExpressionHelper.IsNullOrEmpty((Book b) => b.Publisher))
                //.Where(ExpressionHelper.IsNullOrEmpty<Book>( b => b.Publisher))//或这个写法
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }
    }
}