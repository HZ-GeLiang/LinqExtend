using LinqExtend.EF.Test.EF;
using Microsoft.EntityFrameworkCore;

namespace LinqExtend.EF.Test
{
    [TestClass]
    public class WhereExtensionTest
    {
        [TestMethod]
        public void WhereNotDeleted()
        {
            using TestDbContext db = new TestDbContext();

            {
                var query = db.Students
                .WhereNotDeleted(a => a.IsDel)
                .Select(a => new { a.Id });

                var sql = query.ToQueryString();

                var sql_Result = $@"SELECT [s].[Id]
FROM [Students] AS [s]
WHERE [s].[IsDel] = CAST(0 AS bit)";

                Assert.AreEqual(sql, sql_Result);
            }

            {
                var query = db.Students
                .WhereNotDeleted(a => a.IsDel2)
                .Select(a => new { a.Id });

                var sql = query.ToQueryString();

                var sql_Result = $@"SELECT [s].[Id]
FROM [Students] AS [s]
WHERE (([s].[IsDel2] <> CAST(1 AS bit)) OR ([s].[IsDel2] IS NULL)) OR ([s].[IsDel2] IS NULL)";

                Assert.AreEqual(sql, sql_Result);
            }

            {
                var query = db.Students
                .WhereNotDeleted(a => a.IsDel3)
                .Select(a => new { a.Id });

                var sql = query.ToQueryString();

                var sql_Result = $@"SELECT [s].[Id]
FROM [Students] AS [s]
WHERE [s].[IsDel3] <> 1";

                Assert.AreEqual(sql, sql_Result);
            }

            {
                var query = db.Students
                .WhereNotDeleted(a => a.IsDel4)
                .Select(a => new { a.Id });

                var sql = query.ToQueryString();

                var sql_Result = $@"SELECT [s].[Id]
FROM [Students] AS [s]
WHERE (([s].[IsDel4] <> 1) OR ([s].[IsDel4] IS NULL)) OR ([s].[IsDel4] IS NULL)";

                Assert.AreEqual(sql, sql_Result);
            }
        }

        [TestMethod]
        public void WhereIfHasValue()
        {
            using TestDbContext db = new TestDbContext();

            {
                var query = db.Students
                .WhereIfHasValue(true, a => a.UserName)
                .Select(a => new { a.Id });

                var sql = query.ToQueryString();

                var sql_Result = $@"SELECT [s].[Id]
FROM [Students] AS [s]
WHERE COALESCE([s].[UserName], N'') <> N''";

                Assert.AreEqual(sql, sql_Result);
            }

            {
                var query = db.Students
                .WhereIfHasValue(false, a => a.UserName)
                .Select(a => new { a.Id });

                var sql = query.ToQueryString();

                var sql_Result = $@"SELECT [s].[Id]
FROM [Students] AS [s]";

                Assert.AreEqual(sql, sql_Result);
            }
        }

        [TestMethod]
        public void WhereHasValue()
        {
            using TestDbContext db = new TestDbContext();

            var query = db.Students
                 .WhereHasValue(a => a.UserName)
                 .Select(a => new { a.Id });

            var sql = query.ToQueryString();

            var sql_Result = $@"SELECT [s].[Id]
FROM [Students] AS [s]
WHERE COALESCE([s].[UserName], N'') <> N''";

            Assert.AreEqual(sql, sql_Result);
        }

        [TestMethod]
        public void WhereNoValue()
        {
            using TestDbContext db = new TestDbContext();

            var query = db.Students
                 .WhereNoValue(a => a.UserName)
                 .Select(a => new { a.Id });

            var sql = query.ToQueryString();

            var sql_Result = $@"SELECT [s].[Id]
FROM [Students] AS [s]
WHERE COALESCE([s].[UserName], N'') = N''";

            Assert.AreEqual(sql, sql_Result);
        }
    }
}