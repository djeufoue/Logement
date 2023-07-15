using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddNewFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "PaymentHistories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "NotificationSentForSubscriptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ApartmentNumber",
                table: "NotificationSentForRentPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "NotificationSentForRentPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_CityId",
                table: "PaymentHistories",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSentForSubscriptions_CityId",
                table: "NotificationSentForSubscriptions",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSentForRentPayments_CityId",
                table: "NotificationSentForRentPayments",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSentForRentPayments_Cities_CityId",
                table: "NotificationSentForRentPayments",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSentForSubscriptions_Cities_CityId",
                table: "NotificationSentForSubscriptions",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentHistories_Cities_CityId",
                table: "PaymentHistories",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSentForRentPayments_Cities_CityId",
                table: "NotificationSentForRentPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSentForSubscriptions_Cities_CityId",
                table: "NotificationSentForSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentHistories_Cities_CityId",
                table: "PaymentHistories");

            migrationBuilder.DropIndex(
                name: "IX_PaymentHistories_CityId",
                table: "PaymentHistories");

            migrationBuilder.DropIndex(
                name: "IX_NotificationSentForSubscriptions_CityId",
                table: "NotificationSentForSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_NotificationSentForRentPayments_CityId",
                table: "NotificationSentForRentPayments");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "NotificationSentForSubscriptions");

            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                table: "NotificationSentForRentPayments");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "NotificationSentForRentPayments");
        }
    }
}
