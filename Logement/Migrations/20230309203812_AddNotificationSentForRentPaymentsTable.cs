using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddNotificationSentForRentPaymentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmmountSupposedToPay",
                table: "RentPaymentDatesSchedulars",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "NotificationSentForRentPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AmmountSupposedToPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScheduledDateForRentPayment = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationSentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSentForRentPayments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSentForRentPayments");

            migrationBuilder.DropColumn(
                name: "AmmountSupposedToPay",
                table: "RentPaymentDatesSchedulars");
        }
    }
}
