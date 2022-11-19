using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logement.Migrations
{
    public partial class AddCreatedOnColunm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_Apartments_ApartmentId",
                table: "TenantRentApartments");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_ApartmentId",
                table: "TenantRentApartments");

            migrationBuilder.RenameColumn(
                name: "paymentMethodEnum",
                table: "TenantRentApartments",
                newName: "PaymentMethodEnum");

            migrationBuilder.AlterColumn<long>(
                name: "ApartmentId",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountRemainingForRent",
                table: "TenantRentApartments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaidByTenant",
                table: "TenantRentApartments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaidInAdvance",
                table: "TenantRentApartments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Apartments",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "TenantRentApartmentViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApartmentId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    TenantEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BailId = table.Column<long>(type: "bigint", nullable: false),
                    Contract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    DepositePrice = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    AmountPaidByTenant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfMonthsToPay = table.Column<int>(type: "int", nullable: false),
                    AmountRemainingForRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaidInAdvance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    StartOfContract = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndOfContract = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRentApartmentViewModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_ApartmentId",
                table: "TenantRentApartments",
                column: "ApartmentId",
                unique: true,
                filter: "[ApartmentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_Apartments_ApartmentId",
                table: "TenantRentApartments",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantRentApartments_Apartments_ApartmentId",
                table: "TenantRentApartments");

            migrationBuilder.DropTable(
                name: "TenantRentApartmentViewModel");

            migrationBuilder.DropIndex(
                name: "IX_TenantRentApartments_ApartmentId",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "AmountPaidByTenant",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "AmountPaidInAdvance",
                table: "TenantRentApartments");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodEnum",
                table: "TenantRentApartments",
                newName: "paymentMethodEnum");

            migrationBuilder.AlterColumn<long>(
                name: "ApartmentId",
                table: "TenantRentApartments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AmountRemainingForRent",
                table: "TenantRentApartments",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRentApartments_ApartmentId",
                table: "TenantRentApartments",
                column: "ApartmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantRentApartments_Apartments_ApartmentId",
                table: "TenantRentApartments",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
