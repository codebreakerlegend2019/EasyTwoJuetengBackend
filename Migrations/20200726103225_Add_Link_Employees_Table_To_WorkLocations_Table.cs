using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyTwoJuetengBackend.Migrations
{
    public partial class Add_Link_Employees_Table_To_WorkLocations_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkLocationId",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_WorkLocationId",
                table: "Employees",
                column: "WorkLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_WorkLocations_WorkLocationId",
                table: "Employees",
                column: "WorkLocationId",
                principalTable: "WorkLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_WorkLocations_WorkLocationId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_WorkLocationId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "WorkLocationId",
                table: "Employees");
        }
    }
}
