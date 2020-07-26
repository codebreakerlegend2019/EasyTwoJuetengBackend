using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyTwoJuetengBackend.Migrations
{
    public partial class Add_WorkLocation_Tables_Linked_To_Cities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkLocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Barangay = table.Column<string>(nullable: true),
                    CityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLocations_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkLocations_CityId",
                table: "WorkLocations",
                column: "CityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkLocations");
        }
    }
}
