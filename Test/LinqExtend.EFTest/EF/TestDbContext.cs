using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LinqExtend.EF.Test.EF
{
    public partial class TestDbContext : DbContext
    {
        public TestDbContext()
        {
        }

        public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } //表名默认为 DbSet 的属性名, 即 这里为 Books
        public DbSet<Order> Orders { get; set; }  
        public DbSet<Shop> Shops { get; set; }  
        public DbSet<Student> Students { get; set; }    


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connStr = "Data Source = 127.0.0.1 ; Initial Catalog = LinqExtend2EFTest; Integrated Security = True";
                optionsBuilder.UseSqlServer(connStr);
            }
        }

        //各种配置信息
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);//加载当前程序集中所有实现了IEntityTypeConfiguration接口的类
        }

    }
}
