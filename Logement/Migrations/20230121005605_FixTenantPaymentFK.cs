using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class FixTenantPaymentFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantPaymentStatuses_TenantRentApartments_TenantRentApartmentId",
                table: "TenantPaymentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TenantPaymentStatuses_TenantRentApartmentId",
                table: "TenantPaymentStatuses");

            migrationBuilder.DropColumn(
                name: "TenantRentApartmentId",
                table: "TenantPaymentStatuses");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentStatuses_TenantId",
                table: "TenantPaymentStatuses",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantPaymentStatuses_AspNetUsers_TenantId",
                table: "TenantPaymentStatuses",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantPaymentStatuses_AspNetUsers_TenantId",
                table: "TenantPaymentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_TenantPaymentStatuses_TenantId",
                table: "TenantPaymentStatuses");

            migrationBuilder.AddColumn<long>(
                name: "TenantRentApartmentId",
                table: "TenantPaymentStatuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentStatuses_TenantRentApartmentId",
                table: "TenantPaymentStatuses",
                column: "TenantRentApartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantPaymentStatuses_TenantRentApartments_TenantRentApartmentId",
                table: "TenantPaymentStatuses",
                column: "TenantRentApartmentId",
                principalTable: "TenantRentApartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
