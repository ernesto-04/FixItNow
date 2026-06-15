using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixItNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingRequestIdToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingRequestId",
                table: "Tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingRequestId",
                table: "Tickets");
        }
    }
}
