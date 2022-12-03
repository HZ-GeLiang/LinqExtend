using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinqExtend.EF.Test.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Books",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PubTime = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    BookInfo_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookInfo_AuthorName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Publisher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Publisher2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDel = table.Column<bool>(type: "bit", nullable: true),
                    IsDel2 = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Books", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Books");
        }
    }
}
