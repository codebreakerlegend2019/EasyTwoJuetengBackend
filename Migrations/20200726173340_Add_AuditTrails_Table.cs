using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyTwoJuetengBackend.Migrations
{
    public partial class Add_AuditTrails_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditTrails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Module = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    Activity = table.Column<string>(nullable: true),
                    IsSuccessful = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    UsernameInCharge = table.Column<string>(nullable: true),
                    UserRole = table.Column<string>(nullable: true),
                    DateTimeOccurred = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails");
        }
    }
}
