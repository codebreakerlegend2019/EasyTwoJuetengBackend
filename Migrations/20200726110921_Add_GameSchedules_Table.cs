using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyTwoJuetengBackend.Migrations
{
    public partial class Add_GameSchedules_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    DrawTime = table.Column<TimeSpan>(nullable: false),
                    EntryStart = table.Column<TimeSpan>(nullable: false),
                    EntryEnd = table.Column<TimeSpan>(nullable: false),
                    DateTimeCreated = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSchedules", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSchedules");
        }
    }
}
