using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasTracker.Data.Migrations
{
    public partial class _004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "TicketStatuses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatuses_TicketId",
                table: "TicketStatuses",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketStatuses_Tickets_TicketId",
                table: "TicketStatuses",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketStatuses_Tickets_TicketId",
                table: "TicketStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TicketStatuses_TicketId",
                table: "TicketStatuses");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "TicketStatuses");
        }
    }
}
