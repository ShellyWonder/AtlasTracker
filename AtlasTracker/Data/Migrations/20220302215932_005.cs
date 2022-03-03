using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasTracker.Data.Migrations
{
    public partial class _005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDateOffset",
                table: "Projects",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndDateOffset",
                table: "Projects",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "CreatedDateOffset",
                table: "Projects",
                newName: "CreatedDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Projects",
                newName: "StartDateOffset");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Projects",
                newName: "EndDateOffset");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Projects",
                newName: "CreatedDateOffset");
        }
    }
}
