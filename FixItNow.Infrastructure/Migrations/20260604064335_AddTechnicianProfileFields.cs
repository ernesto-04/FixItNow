using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixItNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "TechnicianProfiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "YearsExperience",
                table: "TechnicianProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "TechnicianProfiles");

            migrationBuilder.DropColumn(
                name: "YearsExperience",
                table: "TechnicianProfiles");
        }
    }
}
