using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyTwoJuetengBackend.Migrations
{
    public partial class Add_Entries_Tables_Link_Employee_Customer_GameMode_GameSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstNumber = table.Column<int>(nullable: false),
                    SecondNumber = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    GameModeId = table.Column<int>(nullable: false),
                    GameScheduleId = table.Column<int>(nullable: false),
                    DateTimeEncoded = table.Column<DateTime>(nullable: false),
                    DateTimeLogged = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entries_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Entries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Entries_GameModes_GameModeId",
                        column: x => x.GameModeId,
                        principalTable: "GameModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Entries_GameSchedules_GameScheduleId",
                        column: x => x.GameScheduleId,
                        principalTable: "GameSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_CustomerId",
                table: "Entries",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_EmployeeId",
                table: "Entries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_GameModeId",
                table: "Entries",
                column: "GameModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_GameScheduleId",
                table: "Entries",
                column: "GameScheduleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");
        }
    }
}
