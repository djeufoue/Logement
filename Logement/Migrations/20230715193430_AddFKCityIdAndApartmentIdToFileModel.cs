using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddFKCityIdAndApartmentIdToFileModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApartmentId",
                table: "FileModel",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "FileModel",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_FileModel_ApartmentId",
                table: "FileModel",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileModel_CityId",
                table: "FileModel",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModel_Apartments_ApartmentId",
                table: "FileModel",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileModel_Cities_CityId",
                table: "FileModel",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModel_Apartments_ApartmentId",
                table: "FileModel");

            migrationBuilder.DropForeignKey(
                name: "FK_FileModel_Cities_CityId",
                table: "FileModel");

            migrationBuilder.DropIndex(
                name: "IX_FileModel_ApartmentId",
                table: "FileModel");

            migrationBuilder.DropIndex(
                name: "IX_FileModel_CityId",
                table: "FileModel");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "FileModel");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "FileModel");
        }
    }
}
