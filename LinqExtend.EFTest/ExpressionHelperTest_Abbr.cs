using LinqExtend.EF.Test.EF;
using LinqExtend.Test;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LinqExtend.EF.ExpressionHelper<Book>;// 利用语法

namespace LinqExtend.EF.Test
{
    /// <summary>
    /// 利用语法,进行简写的示例
    /// </summary>
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
        //                .Where(MakeEqual(b => b.BookInfo, bookDto))
        //                .ToQueryString();
        //    Assert.AreEqual(sql1, sql2);
        //}

        [TestMethod]
        public void IsNullOrEmpty_Test()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNullOrEmpty_Test(ctx);
            
            var sql2 = ctx.Books
                .Where(IsNullOrEmpty(b => b.Publisher))// 利用语法,进行简写
                .ToQueryString();

            Assert.AreEqual(sql1, sql2);
        }

    }
}
