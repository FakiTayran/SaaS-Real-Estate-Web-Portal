using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realEstateManagementDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class userPPAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "userPP",
                table: "AspNetUsers",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userPP",
                table: "AspNetUsers");
        }
    }
}
