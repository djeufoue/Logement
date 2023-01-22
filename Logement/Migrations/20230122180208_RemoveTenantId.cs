using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class RemoveTenantId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentHistories_AspNetUsers_TenantId",
                table: "PaymentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantPaymentStatuses_AspNetUsers_TenantId",
                table: "TenantPaymentStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_AspNetUsers_TenantId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_TenantId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantPaymentStatuses_TenantId",
                table: "TenantPaymentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_PaymentHistories_TenantId",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TenantPaymentStatuses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PaymentHistories");

            migrationBuilder.AddColumn<string>(
                name: "TenantEmail",
                table: "TenantRentApartments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantEmail",
                table: "TenantPaymentStatuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantEmail",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantEmail",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "TenantEmail",
                table: "TenantPaymentStatuses");

            migrationBuilder.DropColumn(
                name: "TenantEmail",
                table: "PaymentHistories");

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                table: "TenantPaymentStatuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                table: "PaymentHistories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_TenantId",
                table: "TenantRentApartments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentStatuses_TenantId",
                table: "TenantPaymentStatuses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_TenantId",
                table: "PaymentHistories",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentHistories_AspNetUsers_TenantId",
                table: "PaymentHistories",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantPaymentStatuses_AspNetUsers_TenantId",
                table: "TenantPaymentStatuses",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_AspNetUsers_TenantId",
                table: "TenantRentApartments",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
