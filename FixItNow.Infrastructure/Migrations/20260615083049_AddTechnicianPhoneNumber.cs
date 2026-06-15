using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixItNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "TechnicianProfiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "TechnicianProfiles");
        }
    }
}
