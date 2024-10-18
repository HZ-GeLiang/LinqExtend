using LinqExtend.EF.Test.EF;
using Microsoft.EntityFrameworkCore;

namespace LinqExtend.EF.Test
{
    [TestClass]
    public class WhereExtensionTest
    {
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