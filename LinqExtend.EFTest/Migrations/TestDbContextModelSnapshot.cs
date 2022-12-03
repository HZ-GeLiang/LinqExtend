﻿// <auto-generated />
using System;
using LinqExtend.EF.Test.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LinqExtend.EF.Test.Migrations
{
    [DbContext(typeof(TestDbContext))]
    partial class TestDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Book", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<bool?>("IsDel")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDel2")
                        .HasColumnType("bit");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<DateTime>("PubTime")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<string>("Publisher")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Publisher2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("T_Books");
                });

            modelBuilder.Entity("Book", b =>
                {
                    b.OwnsOne("BookInfo", "BookInfo", b1 =>
                        {
                            b1.Property<long>("BookId")
                                .HasColumnType("bigint");

                            b1.Property<string>("AuthorName")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("nvarchar(20)");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("BookId");

                            b1.ToTable("T_Books");

                            b1.WithOwner()
                                .HasForeignKey("BookId");
                        });

                    b.Navigation("BookInfo")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
