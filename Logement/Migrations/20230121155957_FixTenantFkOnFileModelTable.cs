using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class FixTenantFkOnFileModelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments",
                column: "BailId");

            migrationBuilder.CreateIndex(
                name: "IX_FileModel_TenantId",
                table: "FileModel",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModel_AspNetUsers_TenantId",
                table: "FileModel",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModel_AspNetUsers_TenantId",
                table: "FileModel");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_FileModel_TenantId",
                table: "FileModel");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments",
                column: "BailId",
                unique: true,
                filter: "[BailId] IS NOT NULL");
        }
    }
}
