using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class Update_CityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_SubscriptionPayments_SubscriptionPaymentId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_SubscriptionPaymentId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "SubscriptionPaymentId",
                table: "Cities");

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "SubscriptionPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPayments_CityId",
                table: "SubscriptionPayments",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionPayments_Cities_CityId",
                table: "SubscriptionPayments",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionPayments_Cities_CityId",
                table: "SubscriptionPayments");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionPayments_CityId",
                table: "SubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "SubscriptionPayments");

            migrationBuilder.AddColumn<long>(
                name: "SubscriptionPaymentId",
                table: "Cities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_SubscriptionPaymentId",
                table: "Cities",
                column: "SubscriptionPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_SubscriptionPayments_SubscriptionPaymentId",
                table: "Cities",
                column: "SubscriptionPaymentId",
                principalTable: "SubscriptionPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
