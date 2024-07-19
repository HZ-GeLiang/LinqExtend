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

        public static string GetSql_IsEmpty_Test2(TestDbContext ctx)
        {
            var sql1 = ctx.Books
              .Where(b => string.IsNullOrEmpty(b.Publisher2))
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

        public static string GetSql_IsNotEmpty_Test2(TestDbContext ctx)
        {
            var sql1 = ctx.Books
              .Where(b => !string.IsNullOrEmpty(b.Publisher2))
              .ToQueryString();
            //注: sql 的 where 子句中 是否进行 null判断是取决于字段是否可空
            return sql1;
        }

        public static string GetSql_IsDeleted_Test1(TestDbContext ctx)
        {
            //bool? 的2种写法
            var sql1 = ctx.Books
              //.Where(b => b.IsDel.HasValue && b.IsDel == true)
              .Where(b => b.IsDel == true) //兼顾 bool? 和 bool
              .ToQueryString();
            return sql1;
        }

        public static string GetSql_IsDeleted_Test2(TestDbContext ctx)
        {
            //bool 共2种写法
            var sql1 = ctx.Books
              //.Where(b => b.IsDel2)
              .Where(b => b.IsDel2 == true)  //兼顾 bool? 和 bool
              .ToQueryString();
            return sql1;
        }

        public static string GetSql_IsDeleted_Test3(TestDbContext ctx)
        {
            //bool? 的2种写法
            var sql1 = ctx.Books
              .Where(b => b.IsDel3 == 1) //兼顾 int? 和 int
              .ToQueryString();
            return sql1;
        }

        public static string GetSql_IsDeleted_Test4(TestDbContext ctx)
        {
            //bool 共2种写法
            var sql1 = ctx.Books
              .Where(b => b.IsDel4 == 1)  //兼顾 int? 和 int
              .ToQueryString();
            return sql1;
        }

        public static string GetSql_IsNotDeleted_Test1(TestDbContext ctx)
        {
            //bool? 的2种写法
            var sql1 = ctx.Books
              //.Where(b => b.IsDel == null && b.IsDel == false)
              .Where(b => b.IsDel != true) //兼顾 bool? 和 bool
              .ToQueryString();
            return sql1;
        }

        public static string GetSql_IsNotDeleted_Test2(TestDbContext ctx)
        {
            //bool 共2种写法
            var sql1 = ctx.Books
              //.Where(b => !b.IsDel2)
              .Where(b => b.IsDel2 != true)//兼顾 bool? 和 bool
              .ToQueryString();
            return sql1;
        }

        public static string GetSql_IsNotDeleted_Test3(TestDbContext ctx)
        {
            //bool? 的2种写法
            var sql1 = ctx.Books
              .Where(b => b.IsDel3 != 1) //兼顾 int? 和 int
              .ToQueryString();
            return sql1;
        }

        public static string GetSql_IsNotDeleted_Test4(TestDbContext ctx)
        {
            //bool 共2种写法
            var sql1 = ctx.Books
              .Where(b => b.IsDel4 != 1)//兼顾 int? 和 int
              .ToQueryString();
            return sql1;
        }
    }
}
