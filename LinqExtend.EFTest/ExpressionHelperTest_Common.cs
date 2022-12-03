using LinqExtend.EF.Test.EF;
using Microsoft.EntityFrameworkCore;

namespace LinqExtend.Test
{
    [TestClass]
    public class ExpressionHelperTest_Common
    {
 
        public static string GetSql_MakeEqual_Test(TestDbContext ctx, BookInfo bookDto)
        {
            var sql1 = ctx.Books
                .Where(b => b.BookInfo.Title == bookDto.Title && b.BookInfo.AuthorName == bookDto.AuthorName)
                .ToQueryString();
            return sql1;
        }

        public static string GetSql_IsEmpty_Test(TestDbContext ctx)
        {
            var sql1 = ctx.Books
              .Where(b => string.IsNullOrEmpty(b.Publisher))
              .ToQueryString();
            //注: sql 的 where 子句中 是否进行 null判断是取决于字段是否可空
            return sql1;
        }

        public static string GetSql_IsNotEmpty_Test(TestDbContext ctx)
        {
            var sql1 = ctx.Books
              .Where(b => !string.IsNullOrEmpty(b.Publisher))
              .ToQueryString();
            //注: sql 的 where 子句中 是否进行 null判断是取决于字段是否可空
            return sql1;
        }


    }
}