using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddNewCollunmToRentPaymentDatesSchedulars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRentPaidForThisDate",
                table: "RentPaymentDatesSchedulars");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountAlreadyPaid",
                table: "RentPaymentDatesSchedulars",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingAmount",
                table: "RentPaymentDatesSchedulars",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentStatus",
                table: "RentPaymentDatesSchedulars",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountAlreadyPaid",
                table: "RentPaymentDatesSchedulars");

            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                table: "RentPaymentDatesSchedulars");

            migrationBuilder.DropColumn(
                name: "RentStatus",
                table: "RentPaymentDatesSchedulars");

            migrationBuilder.AddColumn<bool>(
                name: "IsRentPaidForThisDate",
                table: "RentPaymentDatesSchedulars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
