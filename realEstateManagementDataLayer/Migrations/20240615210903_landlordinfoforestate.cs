using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realEstateManagementDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class landlordinfoforestate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LandLordEmail",
                table: "Estates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LandLordName",
                table: "Estates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LandLordPhone",
                table: "Estates",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandLordEmail",
                table: "Estates");

            migrationBuilder.DropColumn(
                name: "LandLordName",
                table: "Estates");

            migrationBuilder.DropColumn(
                name: "LandLordPhone",
                table: "Estates");
        }
    }
}
