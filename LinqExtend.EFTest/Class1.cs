using LinqExtend.EF.Test.EF;
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
    internal class BookService
    {
        void MakeEqual_Test()
        {
            using TestDbContext ctx = new TestDbContext();

            var bookDto = new BookInfo
            {
                AuthorName = "2022-- select 1",
                Title = "2022-1-1"
            };

            /*
             SELECT [t].[Id], [t].[Price], [t].[PubTime], [t].[BookInfo_AuthorName], [t].[BookInfo_Title]
            FROM [T_Books] AS [t]
            WHERE ([t].[BookInfo_Title] = N'2022-1-1') AND ([t].[BookInfo_AuthorName] = N'2022')
             */
            var sql1 = ctx.Books
                .Where(b => b.BookInfo.Title == bookDto.Title && b.BookInfo.AuthorName == bookDto.AuthorName)
                .ToQueryString();

            var sql2 = ctx.Books
                .Where(MakeEqual(b => b.BookInfo, bookDto))// 利用语法,进行简写
                .ToQueryString();

            Console.WriteLine(sql1);
            Console.WriteLine("==========");
            Console.WriteLine(sql2);

        }

        void IsNullOrEmpty_Test()
        {
            using TestDbContext ctx = new TestDbContext();

            /*
             SELECT [t].[Id], [t].[Price], [t].[PubTime], [t].[Publisher], [t].[BookInfo_AuthorName], [t].[BookInfo_Title]
            FROM [T_Books] AS [t]
            WHERE [t].[Publisher] LIKE N''
            --注: sql 的where  是否 判断 null , 取决于 字段是否可空
            */
            var sql1 = ctx.Books
              .Where(b => string.IsNullOrEmpty(b.Publisher))
              .ToQueryString();

            var sql2 = ctx.Books
                .Where(IsNullOrEmpty(b => b.Publisher))// 利用语法,进行简写
                .ToQueryString();

            Console.WriteLine(sql1);
            Console.WriteLine("==========");
            Console.WriteLine(sql2);

        }

    }
}
