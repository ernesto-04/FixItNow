using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixItNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBookingRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Users_CustomerId",
                table: "BookingRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingRequests_Users_TechnicianId",
                table: "BookingRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingRequests",
                table: "BookingRequests");

            migrationBuilder.RenameTable(
                name: "BookingRequests",
                newName: "booking_requests");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRequests_TechnicianId",
                table: "booking_requests",
                newName: "IX_booking_requests_TechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingRequests_CustomerId",
                table: "booking_requests",
                newName: "IX_booking_requests_CustomerId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "booking_requests",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_booking_requests",
                table: "booking_requests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_requests_Users_CustomerId",
                table: "booking_requests",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_requests_Users_TechnicianId",
                table: "booking_requests",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_requests_Users_CustomerId",
                table: "booking_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_booking_requests_Users_TechnicianId",
                table: "booking_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_booking_requests",
                table: "booking_requests");

            migrationBuilder.RenameTable(
                name: "booking_requests",
                newName: "BookingRequests");

            migrationBuilder.RenameIndex(
                name: "IX_booking_requests_TechnicianId",
                table: "BookingRequests",
                newName: "IX_BookingRequests_TechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_requests_CustomerId",
                table: "BookingRequests",
                newName: "IX_BookingRequests_CustomerId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "BookingRequests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingRequests",
                table: "BookingRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Users_CustomerId",
                table: "BookingRequests",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRequests_Users_TechnicianId",
                table: "BookingRequests",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
