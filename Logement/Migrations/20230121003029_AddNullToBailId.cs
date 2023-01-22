using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddNullToBailId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_FileModel_BailId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments");

            migrationBuilder.AlterColumn<long>(
                name: "BailId",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments",
                column: "BailId",
                unique: true,
                filter: "[BailId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_FileModel_BailId",
                table: "TenantRentApartments",
                column: "BailId",
                principalTable: "FileModel",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_FileModel_BailId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments");

            migrationBuilder.AlterColumn<long>(
                name: "BailId",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_BailId",
                table: "TenantRentApartments",
                column: "BailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_FileModel_BailId",
                table: "TenantRentApartments",
                column: "BailId",
                principalTable: "FileModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
