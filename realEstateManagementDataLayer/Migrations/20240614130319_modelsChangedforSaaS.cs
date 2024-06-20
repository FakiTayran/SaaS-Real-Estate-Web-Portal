using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace realEstateManagementDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class modelsChangedforSaaS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfRooms",
                table: "Estates",
                newName: "SquareMeter");

            migrationBuilder.AddColumn<bool>(
                name: "Balcony",
                table: "Estates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Garden",
                table: "Estates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBathRooms",
                table: "Estates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBedRooms",
                table: "Estates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Estates",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RealEstateCompanyId",
                table: "Estates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RealEstateCompanyId",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RealEstateCompany",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<byte[]>(type: "bytea", nullable: true),
                    TaxNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstateCompany", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estates_RealEstateCompanyId",
                table: "Estates",
                column: "RealEstateCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RealEstateCompanyId",
                table: "AspNetUsers",
                column: "RealEstateCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_RealEstateCompany_RealEstateCompanyId",
                table: "AspNetUsers",
                column: "RealEstateCompanyId",
                principalTable: "RealEstateCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Estates_RealEstateCompany_RealEstateCompanyId",
                table: "Estates",
                column: "RealEstateCompanyId",
                principalTable: "RealEstateCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_RealEstateCompany_RealEstateCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Estates_RealEstateCompany_RealEstateCompanyId",
                table: "Estates");

            migrationBuilder.DropTable(
                name: "RealEstateCompany");

            migrationBuilder.DropIndex(
                name: "IX_Estates_RealEstateCompanyId",
                table: "Estates");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RealEstateCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Balcony",
                table: "Estates");

            migrationBuilder.DropColumn(
                name: "Garden",
                table: "Estates");

            migrationBuilder.DropColumn(
                name: "NumberOfBathRooms",
                table: "Estates");

            migrationBuilder.DropColumn(
                name: "NumberOfBedRooms",
                table: "Estates");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Estates");

            migrationBuilder.DropColumn(
                name: "RealEstateCompanyId",
                table: "Estates");

            migrationBuilder.DropColumn(
                name: "RealEstateCompanyId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SquareMeter",
                table: "Estates",
                newName: "NumberOfRooms");
        }
    }
}
