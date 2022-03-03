using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasTracker.Data.Migrations
{
    public partial class ProjectName_003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "TicketHistories",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "TicketAttachments",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Notifications",
                newName: "CreatedDate");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                type: "character varying(240)",
                maxLength: 240,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "TicketHistories",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "TicketAttachments",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Notifications",
                newName: "Created");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(240)",
                oldMaxLength: 240);
        }
    }
}
