﻿using LinqExtend.EF.Test.EF;
using LinqExtend.Test;
using Microsoft.EntityFrameworkCore;

using static LinqExtend.EF.ExpressionHelper<Book>;// 利用语法

namespace LinqExtend.EF.Test
{
    /// <summary>
    /// 利用语法,进行简写的示例
    /// </summary>
    [TestClass]
    public class ExpressionHelperTest_Abbr
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
        public void IsEmpty_Test()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsEmpty_Test(ctx);

            var sql2 = ctx.Books
                .Where(IsEmpty(b => b.Publisher))// 利用语法,进行简写
            .ToQueryString();

            Assert.AreEqual(sql1, sql2); //([t].[Publisher] IS NULL) OR([t].[Publisher] LIKE N'')
        }

        [TestMethod]
        public void IsEmpty_Test2()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsEmpty_Test2(ctx);

            var sql2 = ctx.Books
                .Where(IsEmpty(b => b.Publisher2))// 利用语法,进行简写
                .ToQueryString();

            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotEmpty_Test()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotEmpty_Test(ctx);

            var sql2 = ctx.Books
                .Where(IsNotEmpty(b => b.Publisher))// 利用语法,进行简写
                .ToQueryString();

            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotEmpty_Test2()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotEmpty_Test2(ctx);

            var sql2 = ctx.Books
                .Where(IsNotEmpty(b => b.Publisher2))// 利用语法,进行简写
                .ToQueryString();

            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsDeleted_Test1()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsDeleted_Test1(ctx);

            var sql2 = ctx.Books
                .Where(IsDeleted(b => b.IsDel))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsDeleted_Test2()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsDeleted_Test2(ctx);

            var sql2 = ctx.Books
                .Where(IsDeleted(b => b.IsDel2))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsDeleted_Test3()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsDeleted_Test3(ctx);

            var sql2 = ctx.Books
                .Where(IsDeleted(b => b.IsDel3))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsDeleted_Test4()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsDeleted_Test4(ctx);

            var sql2 = ctx.Books
                .Where(IsDeleted(b => b.IsDel4))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotDeleted_Test1()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotDeleted_Test1(ctx);

            var sql2 = ctx.Books
                .Where(IsNotDeleted(b => b.IsDel))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotDeleted_Test2()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotDeleted_Test2(ctx);

            var sql2 = ctx.Books
                .Where(IsNotDeleted(b => b.IsDel2))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotDeleted_Test3()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotDeleted_Test3(ctx);

            var sql2 = ctx.Books
                .Where(IsNotDeleted(b => b.IsDel3))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void IsNotDeleted_Test4()
        {
            using TestDbContext ctx = new TestDbContext();
            var sql1 = ExpressionHelperTest_Common.GetSql_IsNotDeleted_Test4(ctx);

            var sql2 = ctx.Books
                .Where(IsNotDeleted(b => b.IsDel4))
                .ToQueryString();
            Assert.AreEqual(sql1, sql2);
        }

        [TestMethod]
        public void SelectMap_DbSet_Test()
        {
            using TestDbContext ctx = new TestDbContext();

            var query1 = ctx.Books.Select(a => new BookDto()
            {
                Id = a.Id,
                PubTime = a.PubTime,
                Price = a.Price,
                Publisher = a.Publisher
            });

            var query2 = ctx.Books.Select(SelectMap<BookDto>());

            var sql1 = query1.ToQueryString();
            var sql2 = query2.ToQueryString();
            Assert.AreEqual(sql1, sql2);

            //var queryList = query1.ToList();
            //var selectMapList = query2.ToList();
            //CollectionAssert.AreEqual(queryList, selectMapList);
        }
    }
}