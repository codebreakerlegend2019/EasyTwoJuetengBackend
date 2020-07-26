using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyTwoJuetengBackend.Migrations
{
    public partial class Add_Nickname_Employee_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NickName",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NickName",
                table: "Employees");
        }
    }
}
