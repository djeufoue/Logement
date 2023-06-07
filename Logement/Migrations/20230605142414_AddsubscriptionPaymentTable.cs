using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddsubscriptionPaymentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SubscriptionPaymentId",
                table: "Cities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "SubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NextPaymentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LandLordId = table.Column<long>(type: "bigint", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPayments", x => x.Id);
                });

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
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_SubscriptionPayments_SubscriptionPaymentId",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "SubscriptionPayments");

            migrationBuilder.DropIndex(
                name: "IX_Cities_SubscriptionPaymentId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "SubscriptionPaymentId",
                table: "Cities");
        }
    }
}
